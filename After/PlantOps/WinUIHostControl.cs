using System.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Hosting;
using PlantOps.Core;
using Windows.Graphics;

namespace PlantOps;

public sealed class WinUIHostControl : Control
{
    private DesktopWindowXamlSource? _source;
    private WinUI.EquipmentStatusPanel? _panel;
    private Equipment? _equipment;
    public event EventHandler<MaintenanceRequestedEventArgs>? MaintenanceRequested;
    public void SetEquipment(Equipment equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);
        if (InvokeRequired) throw new InvalidOperationException("Equipment must be updated on the WinForms UI thread.");
        // Selection can arrive before WinForms creates the HWND. Retain the same
        // domain object so initial creation and HWND recreation show current state.
        _equipment = equipment;
        _panel?.SetEquipment(equipment);
    }

    public WinUIHostControl()
    {
        SetStyle(ControlStyles.Selectable, true);
        TabStop = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
        // This creates a child HWND beneath this WinForms control. Use the public
        // WinUI 3 WindowId API, not the UWP IDesktopWindowXamlSourceNative interface.
        _source = new DesktopWindowXamlSource();
        try
        {
            _source.Initialize(Win32Interop.GetWindowIdFromWindow(Handle));
            _source.TakeFocusRequested += OnTakeFocusRequested;
            _panel = new WinUI.EquipmentStatusPanel();
            if (_equipment is not null) _panel.SetEquipment(_equipment);
            _panel.MaintenanceRequested += OnMaintenanceRequested;
            _source.Content = _panel;
            ResizeIsland();
        }
        catch { ReleaseIsland(); throw; }
    }

    private void OnMaintenanceRequested(object? sender, MaintenanceRequestedEventArgs e)
    {
        // Defer opening a modal WinForms dialog until the WinUI Click stack unwinds.
        // The event carries the clicked machine even if selection changes later.
        BeginInvoke((Action)(() =>
        {
            if (IsDisposed || _panel is null) return;
            MaintenanceRequested?.Invoke(this, e);
            if (!IsDisposed && Visible) _panel?.RestoreMaintenanceFocus();
        }));
    }

    protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); ResizeIsland(); }
    protected override void OnDpiChangedAfterParent(EventArgs e) { base.OnDpiChangedAfterParent(e); ResizeIsland(); }
    private void ResizeIsland()
    {
        // ClientSize is already in physical pixels. WinUI converts those into DIPs
        // for layout; multiplying by DeviceDpi again would scale the island twice.
        _source?.SiteBridge.MoveAndResize(new RectInt32(0, 0, ClientSize.Width, ClientSize.Height));
    }
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        _source?.NavigateFocus(new XamlSourceFocusNavigationRequest(
            ModifierKeys.HasFlag(Keys.Shift) ? XamlSourceFocusNavigationReason.Last : XamlSourceFocusNavigationReason.First));
    }
    private void OnTakeFocusRequested(DesktopWindowXamlSource sender, DesktopWindowXamlSourceTakeFocusRequestedEventArgs args)
    {
        // Return Tab/Shift+Tab to the existing form's tab order, including wrapping.
        FindForm()?.SelectNextControl(this, args.Request.Reason != XamlSourceFocusNavigationReason.Last, true, true, true);
    }
    protected override void OnHandleDestroyed(EventArgs e) { ReleaseIsland(); base.OnHandleDestroyed(e); }
    protected override void Dispose(bool disposing) { if (disposing) ReleaseIsland(); base.Dispose(disposing); }
    private void ReleaseIsland()
    {
        if (_source is null) return;
        if (_panel is not null) _panel.MaintenanceRequested -= OnMaintenanceRequested;
        _source.TakeFocusRequested -= OnTakeFocusRequested;
        _source.Content = null;
        _source.Dispose();
        _source = null;
        _panel = null;
    }
}
