namespace PlantOps.Core;

public enum EquipmentStatus { Operational, MaintenanceRequired, Offline }

public sealed class Equipment
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public EquipmentStatus Status { get; init; }
    public int HealthPercentage { get; init; }
    public DateTime LastServiceDate { get; init; }
    public DateTime NextServiceDate { get; init; }
    public double TemperatureCelsius { get; init; }
    public int RuntimeHours { get; init; }
    public int ActiveAlerts { get; init; }
    public string? CurrentWarning { get; init; }
    public string StatusText => Status == EquipmentStatus.MaintenanceRequired ? "Maintenance required" : Status.ToString();
}

public sealed class MaintenanceRequestedEventArgs(Equipment equipment) : EventArgs
{
    public Equipment Equipment { get; } = equipment;
}
