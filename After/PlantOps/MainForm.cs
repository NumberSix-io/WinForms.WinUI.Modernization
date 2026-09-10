using PlantOps.Core;

namespace PlantOps;

public sealed class MainForm : Form
{
    private readonly WinUIHostControl _details = new() { Dock = DockStyle.Fill };
    private readonly ListBox _equipmentList = new() { Dock = DockStyle.Fill, DisplayMember = nameof(Equipment.Name), IntegralHeight = false, HorizontalScrollbar = true, AccessibleName = "Equipment list" };
    private readonly ToolStripStatusLabel _status = new("Ready • Demonstration data");

    public MainForm()
    {
        Text = "PlantOps — Equipment Management";
        SuspendLayout();
        // All dimensions below are authored at 96 DPI; scale once for the current monitor.
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1280, 720);
        MinimumSize = new Size(1180, 680);
        StartPosition = FormStartPosition.CenterScreen;
        var statusBar = new StatusStrip();
        statusBar.Items.Add(_status);
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var navigation = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(12, 20, 12, 0), BackColor = SystemColors.Control };
        navigation.Controls.Add(new Label { Text = "PlantOps", Font = new Font(Font, FontStyle.Bold), AutoSize = true, Margin = new Padding(3, 0, 3, 28) });
        var content = new Panel { Dock = DockStyle.Fill };
        var equipmentPage = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(16) };
        equipmentPage.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));
        equipmentPage.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        equipmentPage.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        equipmentPage.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var title = new Label { Text = "Equipment", Font = new Font(Font.FontFamily, 17, FontStyle.Bold), AutoSize = true, Margin = new Padding(3, 0, 3, 16) };
        equipmentPage.Controls.Add(title, 0, 0);
        equipmentPage.SetColumnSpan(title, 2);
        equipmentPage.Controls.Add(_equipmentList, 0, 1);
        equipmentPage.Controls.Add(_details, 1, 1);
        var placeholder = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Visible = false };
        content.Controls.Add(equipmentPage);
        content.Controls.Add(placeholder);
        foreach (var section in new[] { "Equipment", "Work Orders", "Maintenance", "Reports", "Settings" })
        {
            var button = new Button { Text = section, Width = 160, Height = 38, AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
            button.Click += (_, _) =>
            {
                equipmentPage.Visible = section == "Equipment";
                placeholder.Visible = !equipmentPage.Visible;
                placeholder.Text = $"{section}\n\nThis existing application area is outside the demo.";
                _status.Text = section;
            };
            navigation.Controls.Add(button);
        }
        shell.Controls.Add(navigation, 0, 0);
        shell.Controls.Add(content, 1, 0);
        Controls.Add(shell);
        Controls.Add(statusBar);
        _details.MaintenanceRequested += OnMaintenanceRequested;
        _equipmentList.SelectedIndexChanged += OnEquipmentSelected;
        _equipmentList.DataSource = new EquipmentRepository().GetAll();
        ResumeLayout(true);
    }

    private void OnEquipmentSelected(object? sender, EventArgs e)
    {
        if (_equipmentList.SelectedItem is Equipment equipment)
        {
            _details.SetEquipment(equipment);
            _status.Text = $"{equipment.Name} • {equipment.Location}";
        }
    }

    private void OnMaintenanceRequested(object? sender, MaintenanceRequestedEventArgs e) => RequestMaintenance(e.Equipment);

    private void RequestMaintenance(Equipment equipment)
    {
        using var dialog = new ScheduleMaintenanceForm(equipment);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _status.Text = $"Maintenance scheduled for {equipment.Name}";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _details.MaintenanceRequested -= OnMaintenanceRequested;
            _equipmentList.SelectedIndexChanged -= OnEquipmentSelected;
        }
        base.Dispose(disposing);
    }
}
