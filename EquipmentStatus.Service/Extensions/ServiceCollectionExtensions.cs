using EquipmentStatus.Service.Core;
using EquipmentStatus.Service.Database;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentStatus.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServiceDependencies(this IServiceCollection services) =>
        services
            .AddSingleton<EquipmentDbContext>()
            .AddTransient<IEquipmentRepository, EquipmentRepository>()
            .AddTransient<IEquipmentStateParser, EquipmentStateParser>();
}