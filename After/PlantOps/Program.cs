using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Hosting;

namespace PlantOps;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        // The package's generated initializer loads the app-local Windows App SDK.
        // XAML, its dispatcher and every HWND below all belong to this STA thread.
        var dispatcher = DispatcherQueueController.CreateOnCurrentThread();
        XamlEnvironment? xamlApp = null;
        WindowsXamlManager? manager = null;
        var filter = new XamlMessageFilter();
        try
        {
            xamlApp = new XamlEnvironment();
            manager = WindowsXamlManager.InitializeForCurrentThread();
            xamlApp.InitializeResources();
            Application.AddMessageFilter(filter);
            using var form = new MainForm();
            if (args.Length == 2 && args[0] == "--verify-host")
                HostVerification.Attach(form, Path.GetFullPath(args[1]));
            Application.Run(form);
        }
        finally
        {
            // Dispose forms/islands before XAML; ShutdownQueue pumps the remaining
            // dispatcher work synchronously. Blocking ShutdownQueueAsync would deadlock.
            Application.RemoveMessageFilter(filter);
            manager?.Dispose();
            dispatcher.ShutdownQueue();
            GC.KeepAlive(xamlApp);
        }
    }
}
