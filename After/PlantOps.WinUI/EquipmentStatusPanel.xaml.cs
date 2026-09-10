using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PlantOps.Core;

namespace PlantOps.WinUI;

public sealed partial class EquipmentStatusPanel : UserControl
{
    private Equipment? _equipment;
    public event EventHandler<MaintenanceRequestedEventArgs>? MaintenanceRequested;

    public EquipmentStatusPanel() => InitializeComponent();

    public void SetEquipment(Equipment equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);
        _equipment = equipment;
        EquipmentName.Text = equipment.Name;
        Identity.Text = $"{equipment.SerialNumber}  •  {equipment.Location}";
        Status.Text = equipment.StatusText;
        Health.Value = equipment.HealthPercentage;
        HealthText.Text = $"{equipment.HealthPercentage}%";
        NextService.Text = equipment.NextServiceDate.ToString("d MMMM yyyy");
        LastService.Text = $"Last serviced {equipment.LastServiceDate:d MMMM yyyy}";
        Temperature.Text = $"{equipment.TemperatureCelsius:0.#} °C";
        Runtime.Text = $"{equipment.RuntimeHours:N0} h";
        Alerts.Text = equipment.ActiveAlerts.ToString();
        Warning.Message = equipment.CurrentWarning ?? string.Empty;
        Warning.IsOpen = !string.IsNullOrWhiteSpace(equipment.CurrentWarning);
        ScheduleButton.IsEnabled = true;
    }

    private void OnScheduleClick(object sender, RoutedEventArgs e)
    {
        if (_equipment is not null) MaintenanceRequested?.Invoke(this, new(_equipment));
    }

    public void RestoreMaintenanceFocus() => ScheduleButton.Focus(FocusState.Programmatic);
}
