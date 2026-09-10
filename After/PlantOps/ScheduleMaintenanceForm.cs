using PlantOps.Core;

namespace PlantOps;

public sealed class ScheduleMaintenanceForm : Form
{
    public ScheduleMaintenanceForm(Equipment equipment)
    {
        Text = "Schedule maintenance";
        SuspendLayout();
        // All dimensions below are authored at 96 DPI; scale once for the current monitor.
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(510, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = MinimizeBox = false;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 5 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var type = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = "Maintenance type" };
        type.Items.AddRange(["Preventive service", "Inspection", "Repair", "Calibration"]);
        type.SelectedIndex = 0;
        var date = new DateTimePicker { Dock = DockStyle.Fill, MinDate = DateTime.Today, Value = DateTime.Today.AddDays(1), Format = DateTimePickerFormat.Short, AccessibleName = "Requested date" };
        var notes = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, AccessibleName = "Engineer notes" };
        string[] labels = ["Equipment", "Maintenance type", "Requested date", "Engineer notes"];
        Control[] inputs = [new Label { Text = equipment.Name, AutoSize = true, MaximumSize = new Size(305, 0) }, type, date, notes];
        for (var row = 0; row < labels.Length; row++)
        {
            layout.RowStyles.Add(new RowStyle(row == 3 ? SizeType.Percent : SizeType.Absolute, row == 3 ? 100 : 52));
            layout.Controls.Add(new Label { Text = labels[row], AutoSize = true, Padding = new Padding(0, 5, 0, 0) }, 0, row);
            layout.Controls.Add(inputs[row], 1, row);
        }
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
        var cancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
        var schedule = new Button { Text = "Schedule", AutoSize = true };
        schedule.Click += (_, _) =>
        {
            MessageBox.Show(this, $"Maintenance scheduled for {equipment.Name}", "PlantOps", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        };
        buttons.Controls.AddRange([cancel, schedule]);
        layout.Controls.Add(buttons, 0, 4);
        layout.SetColumnSpan(buttons, 2);
        Controls.Add(layout);
        AcceptButton = schedule;
        CancelButton = cancel;
        ResumeLayout(true);
    }
}
