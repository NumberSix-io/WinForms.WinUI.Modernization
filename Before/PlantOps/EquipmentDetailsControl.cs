using PlantOps.Core;

namespace PlantOps;

public sealed class EquipmentDetailsControl : UserControl
{
    private Equipment? _equipment;
    private readonly Label[] _values = Enumerable.Range(0, 10).Select(_ => new Label { AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }).ToArray();
    private readonly ProgressBar _health = new() { Dock = DockStyle.Fill };
    private readonly Label _warning = new() { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(0, 12, 0, 12) };
    private readonly Button _schedule = new() { Text = "Schedule maintenance", AutoSize = true, Enabled = false };
    public event EventHandler<MaintenanceRequestedEventArgs>? MaintenanceRequested;

    public EquipmentDetailsControl()
    {
        SuspendLayout();
        // All dimensions below are authored at 96 DPI; scale once for the current monitor.
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScroll = true;
        Padding = new Padding(22);
        var group = new GroupBox { Text = "Equipment details", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(18) };
        var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        string[] names = ["Equipment", "Serial number", "Location", "Status", "Last service", "Next service", "Health", "Temperature", "Runtime", "Active alerts"];
        for (var i = 0; i < names.Length; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            table.Controls.Add(new Label { Text = names[i], AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, i);
            table.Controls.Add(_values[i], 1, i);
        }
        table.Controls.Add(_health, 1, 10);
        table.Controls.Add(_warning, 0, 11);
        table.SetColumnSpan(_warning, 2);
        table.Controls.Add(_schedule, 1, 12);
        _schedule.Click += OnMaintenanceClick;
        group.Controls.Add(table);
        Controls.Add(group);
        ResumeLayout(true);
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;
        string[] values = [equipment.Name, equipment.SerialNumber, equipment.Location, equipment.StatusText,
            equipment.LastServiceDate.ToString("d MMMM yyyy"), equipment.NextServiceDate.ToString("d MMMM yyyy"),
            $"{equipment.HealthPercentage}%", $"{equipment.TemperatureCelsius:0.#} °C", $"{equipment.RuntimeHours:N0} hours", equipment.ActiveAlerts.ToString()];
        for (var i = 0; i < values.Length; i++) _values[i].Text = values[i];
        _health.Value = equipment.HealthPercentage;
        _warning.Text = equipment.CurrentWarning ?? "No active warnings";
        _schedule.Enabled = true;
    }

    private void OnMaintenanceClick(object? sender, EventArgs e)
    {
        if (_equipment is not null) MaintenanceRequested?.Invoke(this, new(_equipment));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) _schedule.Click -= OnMaintenanceClick;
        base.Dispose(disposing);
    }
}
