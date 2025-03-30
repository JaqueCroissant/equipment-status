using AutoFixture;
using AutoFixture.Xunit2;
using EquipmentStatus.Service.Core;

namespace EquipmentStatus.Service.Tests;

public class EquipmentStateParserTest
{
    [Theory]
    [AutoData]
    public void Should_return_parsed_equipment_state_when_all_parameters_are_valid(
        IFixture fixture,
        EquipmentStateParser sut)
    {
        var now = DateTimeOffset.Now.AddSeconds(-1);
        var timestamp = Trim(now,TimeSpan.TicksPerSecond);
        
        var identifier = fixture.Create<string>();
        var operationalState = fixture.Create<OperationalState>();
        
        
        var expected = new EquipmentState(identifier.ToUpperInvariant(), operationalState, timestamp);
        
        var actual = sut.Validate(identifier, operationalState.ToString(),  timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
        
        Assert.Equivalent(expected, actual);
    }
    
    [Theory]
    [AutoData]
    public void Should_return_null_if_identifier_is_empty_string(
        IFixture fixture,
        EquipmentStateParser sut)
    {
        var now = DateTimeOffset.Now.AddSeconds(-1);
        var timestamp = Trim(now,TimeSpan.TicksPerSecond);
        
        var operationalState = fixture.Create<OperationalState>();
        
        var actual = sut.Validate(string.Empty, operationalState.ToString(),  timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
        
        Assert.Null(actual);
    }
    
    [Theory]
    [AutoData]
    public void Should_return_null_if_operational_state_is_not_a_valid_enum_value(
        IFixture fixture,
        EquipmentStateParser sut)
    {
        var now = DateTimeOffset.Now.AddSeconds(-1);
        var timestamp = Trim(now,TimeSpan.TicksPerSecond);

        var identifier = fixture.Create<string>();
        var operationalState = fixture.Create<string>();
        
        var actual = sut.Validate(identifier, operationalState,  timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
        
        Assert.Null(actual);
    }
    
    [Theory]
    [AutoData]
    public void Should_return_null_if_timestamp_is_not_valid(
        IFixture fixture,
        EquipmentStateParser sut)
    {
        var timestamp = fixture.Create<string>();
        var identifier = fixture.Create<string>();
        var operationalState = fixture.Create<OperationalState>();
        
        var actual = sut.Validate(identifier, operationalState.ToString(),  timestamp);
        
        Assert.Null(actual);
    }
    
    [Theory]
    [AutoData]
    public void Should_return_null_if_identifier_is_whitespace(
        IFixture fixture,
        EquipmentStateParser sut)
    {
        var now = DateTimeOffset.Now.AddSeconds(-1);
        var timestamp = Trim(now,TimeSpan.TicksPerSecond);
        
        var operationalState = fixture.Create<OperationalState>();
        
        var actual = sut.Validate(" ", operationalState.ToString(),  timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
        
        Assert.Null(actual);
    }
    
    private static DateTimeOffset Trim(DateTimeOffset dateTime, long ticks) {
        return new DateTimeOffset(dateTime.Ticks - dateTime.Ticks % ticks, dateTime.Offset);
    }
}