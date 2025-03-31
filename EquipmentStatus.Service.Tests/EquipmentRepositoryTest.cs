using AutoFixture;
using AutoFixture.Xunit2;
using EntityFrameworkCore.AutoFixture.InMemory;
using EquipmentStatus.Service.Core;
using EquipmentStatus.Service.Database;

namespace EquipmentStatus.Service.Tests;

public class EquipmentRepositoryTest
{
    [Theory]
    [InMemoryData]
    public async Task Should_upsert_equipment_state(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var sut = new EquipmentRepository(dbContext);
        
        var input = fixture.Create<EquipmentState>();
        await sut.Upsert(input);

        var actual = dbContext.EquipmentStates.Single();
        
        Assert.NotNull(actual);
        Assert.Equal(input.EquipmentIdentifier, actual.EquipmentIdentifier);
        Assert.Equal(input.TimeStamp, actual.TimeStamp);
        Assert.Equal(input.State.ToString(), actual.State);
    }
    
    [Theory]
    [InMemoryData]
    public async Task Should_return_latest_equipment_state(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var latestEquipmentStates = fixture.CreateMany<EquipmentState>(3).ToArray();
        var olderEquipmentStates = new[]
        {
            CreateOlderEquipmentState(latestEquipmentStates[0]),
            CreateOlderEquipmentState(latestEquipmentStates[1]),
            CreateOlderEquipmentState(latestEquipmentStates[2]),
        };
        
        var allEquipmentStates = latestEquipmentStates
            .Concat(olderEquipmentStates)
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        await dbContext.AddRangeAsync(allEquipmentStates);
        
        var sut = new EquipmentRepository(dbContext);

        var expected = latestEquipmentStates.Select(ToEquipmentStateModel).ToArray();
        var actual = await sut.GetLatestStates();
        
        Assert.All(actual, x =>
        {
            Assert.Single(
                expected
                    .Where(expectedItem => 
                        expectedItem.EquipmentIdentifier
                            .Equals(x.EquipmentIdentifier, StringComparison.InvariantCultureIgnoreCase)
                        && expectedItem.TimeStamp == x.TimeStamp
                        && expectedItem.State
                            .Equals(x.State.ToString(), StringComparison.InvariantCultureIgnoreCase)));
        });
    }
    
    [Theory]
    [InMemoryData]
    public async Task Should_return_history_for_multiple_equipment_states_within_given_time_period(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        
        var equipmentStates = fixture
            .Build<EquipmentState>()
            .With(x => x.TimeStamp, now)
            .CreateMany(3)
            .ToList();
        
        var olderEquipmentStates = new[]
        {
            CreateOlderEquipmentState(equipmentStates[0]),
            CreateOlderEquipmentState(equipmentStates[1], 2),
            CreateOlderEquipmentState(equipmentStates[2], 3),
        };
        
        var allEquipmentStates = equipmentStates
            .Concat(olderEquipmentStates)
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        await dbContext.AddRangeAsync(allEquipmentStates);
        
        var sut = new EquipmentRepository(dbContext);

        var expected = equipmentStates
            .Concat([olderEquipmentStates[0]])
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        var actual = await sut.GetHistory(now.AddDays(-1), now);
        
        Assert.All(actual, x =>
        {
            Assert.Single(
                expected
                    .Where(expectedItem => 
                        expectedItem.EquipmentIdentifier
                            .Equals(x.EquipmentIdentifier, StringComparison.InvariantCultureIgnoreCase)
                        && expectedItem.TimeStamp == x.TimeStamp
                        && expectedItem.State
                            .Equals(x.State.ToString(), StringComparison.InvariantCultureIgnoreCase)));
        });
    }
    
    [Theory]
    [InMemoryData]
    public async Task Should_return_empty_array_if_no_history_within_dates(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        
        var equipmentStates = fixture
            .Build<EquipmentState>()
            .With(x => x.TimeStamp, now.AddDays(-2))
            .CreateMany(3)
            .ToList();
        
        var olderEquipmentStates = new[]
        {
            CreateOlderEquipmentState(equipmentStates[0]),
            CreateOlderEquipmentState(equipmentStates[1], 2),
            CreateOlderEquipmentState(equipmentStates[2], 3),
        };
        
        var allEquipmentStates = equipmentStates
            .Concat(olderEquipmentStates)
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        await dbContext.AddRangeAsync(allEquipmentStates);
        
        var sut = new EquipmentRepository(dbContext);
        
        var actual = await sut.GetHistory(now.AddDays(-1), now);
        
        Assert.Empty(actual);
    }
    
    [Theory]
    [InMemoryData]
    public async Task Should_return_history_for_specific_equipment_identifier_within_given_time_period(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        
        var equipmentStates = fixture
            .Build<EquipmentState>()
            .With(x => x.TimeStamp, now)
            .CreateMany(3)
            .ToList();
        
        var olderEquipmentStates = new[]
        {
            CreateOlderEquipmentState(equipmentStates[0]),
            CreateOlderEquipmentState(equipmentStates[1], 2),
            CreateOlderEquipmentState(equipmentStates[2], 3),
        };
        
        var allEquipmentStates = equipmentStates
            .Concat(olderEquipmentStates)
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        await dbContext.AddRangeAsync(allEquipmentStates);
        
        var sut = new EquipmentRepository(dbContext);

        var equipmentIdentifier = equipmentStates[0].EquipmentIdentifier;
        
        var expected = new[]
        {
            equipmentStates[0],
            olderEquipmentStates[0]
        };
        
        var actual = await sut.GetHistoryById(equipmentIdentifier, now.AddDays(-1), now);
        
        Assert.All(actual, x =>
        {
            Assert.Single(
                expected
                    .Where(expectedItem => 
                        expectedItem.EquipmentIdentifier
                            .Equals(x.EquipmentIdentifier, StringComparison.InvariantCultureIgnoreCase)
                        && expectedItem.TimeStamp == x.TimeStamp
                        && expectedItem.State == x.State));
        });
    }
    
    [Theory]
    [InMemoryData]
    public async Task Should_return_empty_array_if_no_history_for_equipment_identifier_within_dates(
        IFixture fixture,
        [Frozen] EquipmentDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        
        var equipmentStates = fixture
            .Build<EquipmentState>()
            .With(x => x.TimeStamp, now.AddDays(-2))
            .CreateMany(3)
            .ToList();
        
        var olderEquipmentStates = new[]
        {
            CreateOlderEquipmentState(equipmentStates[0]),
            CreateOlderEquipmentState(equipmentStates[1], 2),
            CreateOlderEquipmentState(equipmentStates[2], 3),
        };
        
        var allEquipmentStates = equipmentStates
            .Concat(olderEquipmentStates)
            .Select(ToEquipmentStateModel)
            .ToArray();
        
        await dbContext.AddRangeAsync(allEquipmentStates);
        
        var equipmentIdentifier = equipmentStates[0].EquipmentIdentifier;
        
        var sut = new EquipmentRepository(dbContext);
        
        var actual = await sut.GetHistoryById(equipmentIdentifier, now.AddDays(-1), now);
        
        Assert.Empty(actual);
    }
    
    private class InMemoryDataAttribute() : AutoDataAttribute(() => 
        new Fixture().Customize(new InMemoryCustomization()));

    private static EquipmentState CreateOlderEquipmentState(EquipmentState current, int daysIntoThePast = 1) => 
        current with { TimeStamp = current.TimeStamp.AddDays(-daysIntoThePast) };

    private static EquipmentStateModel ToEquipmentStateModel(EquipmentState equipmentState) =>
        new()
        {
            EquipmentIdentifier = equipmentState.EquipmentIdentifier,
            TimeStamp = equipmentState.TimeStamp,
            State = equipmentState.State.ToString(),
        };
}