namespace EquipmentStatus.Service.Database;

public class EquipmentStateModel
{
    public Guid Id { get; set; }
    public string EquipmentIdentifier { get; set; } = string.Empty; 
    public string State { get; set; } = string.Empty;

    public DateTimeOffset TimeStamp { get; set; }
}