namespace EquipmentStatus.Service.Core;

public record EquipmentState(string EquipmentIdentifier, OperationalState State, DateTimeOffset TimeStamp);