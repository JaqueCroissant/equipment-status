using Microsoft.EntityFrameworkCore;

namespace EquipmentStatus.Service.Database;

public class EquipmentDbContext : DbContext
{
    public DbSet<EquipmentStateModel> EquipmentStates { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentStateModel>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<EquipmentStateModel>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }

    public void SeedData()
    {
        if (!Database.EnsureCreated())
        {
            return;
        }

        AddRange(_seedData);
        SaveChanges();
    }
    
    private readonly EquipmentStateModel[] _seedData =
    [
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "2_BY_4_PLATE_MACHINE",
            State = "Stopped",
            TimeStamp = DateTime.UtcNow.AddMinutes(-5)
        },
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "2_BY_4_PLATE_MACHINE",
            State = "Transitioning",
            TimeStamp = DateTime.UtcNow.AddMinutes(-3)
        },
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "2_BY_4_PLATE_MACHINE",
            State = "Running",
            TimeStamp = DateTime.UtcNow.AddMinutes(-2)
        },
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "MINIFIGURE_HEAD_MACHINE",
            State = "Running",
            TimeStamp = DateTime.UtcNow.AddMinutes(-10)
        },
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "MINIFIGURE_HEAD_MACHINE",
            State = "Transitioning",
            TimeStamp = DateTime.UtcNow.AddMinutes(-5)
        },
        new()
        {
            Id = Guid.NewGuid(),
            EquipmentIdentifier = "MINIFIGURE_HEAD_MACHINE",
            State = "Stopped",
            TimeStamp = DateTime.UtcNow.AddMinutes(-3)
        }
    ];
}