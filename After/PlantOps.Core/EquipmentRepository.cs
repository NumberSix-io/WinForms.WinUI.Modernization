namespace PlantOps.Core;

public sealed class EquipmentRepository
{
    public IReadOnlyList<Equipment> GetAll() =>
    [
        Create(1, "CNC Mill 04", "MX-4821", "Production Hall A", EquipmentStatus.Operational, 82, 64, 1284, 2, "Spindle vibration above normal range"),
        Create(2, "Laser Cutter 02", "LC-2109", "Fabrication Bay", EquipmentStatus.Operational, 96, 38, 742, 0, null),
        Create(3, "Hydraulic Press 07", "HP-7730", "Production Hall B", EquipmentStatus.MaintenanceRequired, 54, 81, 6210, 3, "Hydraulic pressure fluctuating; inspection recommended"),
        Create(4, "Coordinate Measuring Machine 01", "CM-1008", "Quality Laboratory", EquipmentStatus.Offline, 68, 21, 3210, 1, "Offline pending calibration"),
        Create(5, "Packaging Line 03", "PL-3092", "Dispatch", EquipmentStatus.Operational, 91, 42, 2450, 0, null)
    ];

    private static Equipment Create(int id, string name, string serial, string location, EquipmentStatus status,
        int health, double temperature, int hours, int alerts, string? warning) => new()
        {
            Id = id, Name = name, SerialNumber = serial, Location = location, Status = status,
            HealthPercentage = health, TemperatureCelsius = temperature, RuntimeHours = hours,
            ActiveAlerts = alerts, CurrentWarning = warning,
            LastServiceDate = new DateTime(2026, 6, 12).AddDays(id - 1),
            NextServiceDate = new DateTime(2026, 12, 12).AddDays(id - 1)
        };
}
