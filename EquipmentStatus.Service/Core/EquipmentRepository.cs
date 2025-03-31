using EquipmentStatus.Service.Database;
using Microsoft.EntityFrameworkCore;

namespace EquipmentStatus.Service.Core;

public interface IEquipmentRepository
{
    public Task Upsert(EquipmentState equipmentState);
    public Task<IReadOnlyCollection<EquipmentState>> GetLatestStates();
    public Task<IReadOnlyCollection<EquipmentState>> GetHistory(DateTimeOffset from, DateTimeOffset to);
    public Task<IReadOnlyCollection<EquipmentState>> GetHistoryById(string equipmentIdentifier, DateTimeOffset from, DateTimeOffset to);
}

public class EquipmentRepository : IEquipmentRepository
{
    private readonly EquipmentDbContext _dbContext;
    
    public EquipmentRepository(EquipmentDbContext dbContext)
    {
        dbContext.SeedData();
        _dbContext = dbContext;
    }
    
    public async Task Upsert(EquipmentState equipmentState)
    {
        var model = ToEquipmentStateModel(equipmentState);
        
        _dbContext.EquipmentStates.Add(model);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<EquipmentState>> GetLatestStates()
    { 
        var equipmentStates = await _dbContext
            .EquipmentStates
            .GroupBy(x => x.EquipmentIdentifier)
            .Select(g => 
                g.OrderByDescending(e => e.TimeStamp)
                .First())
            .ToArrayAsync();
        
        return equipmentStates.Select(ToEquipmentState).ToArray();
    }
       

    public async Task<IReadOnlyCollection<EquipmentState>> GetHistory(DateTimeOffset from, DateTimeOffset to)
    {
        var equipmentStates = await _dbContext.EquipmentStates
            .Where(x => 
                x.TimeStamp >= from 
                && x.TimeStamp <= to)
            .OrderByDescending(e => e.TimeStamp)
            .ToArrayAsync();

        return equipmentStates.Select(ToEquipmentState).ToArray();
    }
        
    public async Task<IReadOnlyCollection<EquipmentState>> GetHistoryById(string equipmentIdentifier, DateTimeOffset from, DateTimeOffset to)
    {
        if (string.IsNullOrEmpty(equipmentIdentifier))
        {
            return Array.Empty<EquipmentState>();
        }
        
        var equipmentStates = await _dbContext.EquipmentStates
            .Where(x => 
                x.EquipmentIdentifier.Equals(equipmentIdentifier, 
                    StringComparison.InvariantCultureIgnoreCase) 
                && x.TimeStamp >= from 
                && x.TimeStamp <= to)
            .OrderByDescending(e => e.TimeStamp)
            .ToArrayAsync();
        
        return equipmentStates.Select(ToEquipmentState).ToArray();
    }
    
    private static EquipmentState ToEquipmentState(EquipmentStateModel model) =>
        new (model.EquipmentIdentifier, Enum.Parse<OperationalState>(model.State), model.TimeStamp); 
    
    private static EquipmentStateModel ToEquipmentStateModel(EquipmentState equipmentState) =>
        new()
        {
            EquipmentIdentifier = equipmentState.EquipmentIdentifier,
            State = equipmentState.State.ToString(),
            TimeStamp = equipmentState.TimeStamp
        };
    
}