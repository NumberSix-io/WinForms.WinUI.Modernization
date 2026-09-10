using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using PlantOps.Core;
using Forms = System.Windows.Forms;

namespace PlantOps;

// Opt-in integration checks, run by --verify-host <report-path>. These execute on
// the real STA/message loop and create real islands and owned WinForms dialogs.
// They are kept separate from the product UI and require no test framework.
internal static class HostVerification
{
    internal static void Attach(MainForm form, string reportPath)
    {
        form.Shown += async (_, _) =>
        {
            var results = new List<string>();
            try
            {
                await Task.Delay(200);
                var host = Descendants(form).OfType<WinUIHostControl>().Single();
                var list = Descendants(form).OfType<Forms.ListBox>().Single();
                var machines = (IReadOnlyList<Equipment>)list.DataSource!;
                for (var index = 0; index < machines.Count; index++)
                {
                    list.SelectedIndex = index;
                    var equipment = machines[index];
                    var panel = Field<WinUI.EquipmentStatusPanel>(host, "_panel");
                    Check(((TextBlock)panel.FindName("EquipmentName")).Text == equipment.Name, "Selected name");
                    Check(((TextBlock)panel.FindName("HealthText")).Text == $"{equipment.HealthPercentage}%", "Selected health");
                    Check(Field<InfoBar>(panel, "Warning").IsOpen == (equipment.CurrentWarning is not null), "Warning visibility");
                    Check(((TextBlock)panel.FindName("Runtime")).Text == $"{equipment.RuntimeHours:N0} h", "Runtime");
                    await CheckCancelDialog(form, panel, equipment);
                    results.Add($"PASS selection and owned maintenance dialog/cancel: {equipment.Name}");
                }

                list.SelectedIndex = 0;
                var observedSizes = new HashSet<Size>();
                foreach (var size in new[] { new Size(1180, 680), new Size(1450, 820), new Size(1280, 720) })
                {
                    form.ClientSize = new Size(size.Width * form.DeviceDpi / 96, size.Height * form.DeviceDpi / 96);
                    await Task.Delay(100);
                    var source = Field<DesktopWindowXamlSource>(host, "_source");
                    var hwnd = Win32Interop.GetWindowFromWindowId(source.SiteBridge.WindowId);
                    Check(GetParent(hwnd) == host.Handle, "Island is a native child of the host");
                    Check(GetClientRect(hwnd, out var bounds), "Read island bounds");
                    Check(bounds.Right == host.ClientSize.Width && bounds.Bottom == host.ClientSize.Height, "Island matches host pixels");
                    observedSizes.Add(host.ClientSize);
                    results.Add($"PASS embedded HWND and resize: {bounds.Right} × {bounds.Bottom} pixels, host DPI {host.DeviceDpi}");
                }
                Check(observedSizes.Count >= 2, "Resize test must exercise different actual dimensions");

                var oldPanel = Field<WinUI.EquipmentStatusPanel>(host, "_panel");
                typeof(Forms.Control).GetMethod("RecreateHandle", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, null);
                await Task.Delay(100);
                var newPanel = Field<WinUI.EquipmentStatusPanel>(host, "_panel");
                Check(!ReferenceEquals(oldPanel, newPanel), "New visual tree after HWND recreation");
                Check(((TextBlock)newPanel.FindName("EquipmentName")).Text == machines[0].Name, "Selection survives HWND recreation");
                await CheckCancelDialog(form, newPanel, machines[0]);
                results.Add("PASS HWND recreation preserves selection and one working maintenance subscription");

                form.Activate();
                host.Focus();
                await Task.Delay(100);
                var button = (Microsoft.UI.Xaml.Controls.Button)newPanel.FindName("ScheduleButton");
                Check(button.FocusState != FocusState.Unfocused, "WinForms focus enters WinUI");
                results.Add("PASS WinForms focus enters WinUI maintenance button");
                host.Dispose();
                Check(Field<object?>(host, "_source") is null && Field<object?>(host, "_panel") is null, "Island references released");
                results.Add("PASS deterministic island disposal");
            }
            catch (Exception exception)
            {
                results.Add($"FAIL {exception}");
                Environment.ExitCode = 1;
            }
            finally
            {
                File.WriteAllLines(reportPath, results);
                form.Close();
            }
        };
    }

    private static async Task CheckCancelDialog(MainForm owner, WinUI.EquipmentStatusPanel panel, Equipment equipment)
    {
        Exception? failure = null;
        var completed = new TaskCompletionSource();
        using var timer = new Forms.Timer { Interval = 50 };
        var deadline = DateTime.UtcNow.AddSeconds(5);
        timer.Tick += (_, _) =>
        {
            var dialog = Forms.Application.OpenForms.OfType<ScheduleMaintenanceForm>().FirstOrDefault();
            if (dialog is null)
            {
                if (DateTime.UtcNow < deadline) return;
                timer.Stop();
                failure = new InvalidOperationException("Maintenance dialog did not open.");
                completed.TrySetResult();
                return;
            }
            timer.Stop();
            try
            {
                Check(dialog.Owner == owner, "Maintenance dialog owner");
                Check(Descendants(dialog).OfType<Forms.Label>().Any(l => l.Text == equipment.Name), "Maintenance targets selected machine");
                dialog.CancelButton!.PerformClick();
            }
            catch (Exception exception) { failure = exception; dialog.Close(); }
            completed.TrySetResult();
        };
        timer.Start();
        var button = (Microsoft.UI.Xaml.Controls.Button)panel.FindName("ScheduleButton");
        var peer = new ButtonAutomationPeer(button);
        ((IInvokeProvider)peer.GetPattern(PatternInterface.Invoke)).Invoke();
        await completed.Task;
        await Task.Delay(100); // Allow ShowDialog and deferred focus restoration to unwind.
        if (failure is not null) throw failure;
        Check(!Forms.Application.OpenForms.OfType<ScheduleMaintenanceForm>().Any(), "Cancel closes dialog");
        Check(button.FocusState != FocusState.Unfocused, "Focus returns to the WinUI button after cancellation");
    }

    private static IEnumerable<Forms.Control> Descendants(Forms.Control parent) =>
        parent.Controls.Cast<Forms.Control>().SelectMany(c => new[] { c }.Concat(Descendants(c)));
    private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(owner)!;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")]
    private static extern nint GetParent(nint hwnd);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint hwnd, out Rect rect);
}
