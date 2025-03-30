using System.Globalization;

namespace EquipmentStatus.Service.Core;

public interface IEquipmentStateParser
{
    public EquipmentState? Validate(string equipmentIdentifier, string state, string timestamp);
}

public class EquipmentStateParser : IEquipmentStateParser
{
    public EquipmentState? Validate(string equipmentIdentifier, string state, string timestamp)
    {
        if (!string.IsNullOrWhiteSpace(equipmentIdentifier)
            && HasValidTimestamp(timestamp, out var validTimestamp)
            && HasValidOperationalState(state, out var validOperationalState))
        {
            return new EquipmentState(
                equipmentIdentifier.ToUpperInvariant(), 
                validOperationalState, 
                validTimestamp);
        }

        return null;
    }

    private static bool HasValidTimestamp(string timestamp, out DateTimeOffset parsedTimestamp) =>
        DateTimeOffset.TryParse(timestamp, CultureInfo.InvariantCulture, out parsedTimestamp)
        && parsedTimestamp < DateTimeOffset.UtcNow;

    private static bool HasValidOperationalState(string state, out OperationalState parsedOperationalState) =>
        Enum.TryParse(state, true, out parsedOperationalState);
}