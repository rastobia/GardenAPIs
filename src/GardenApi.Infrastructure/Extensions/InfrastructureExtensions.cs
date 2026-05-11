using GardenApi.Domain.Interfaces;
using GardenApi.Infrastructure.Data;
using GardenApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GardenApi.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<GardenDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IGardenRepository, GardenRepository>();
        services.AddScoped<IPlantRepository, PlantRepository>();
        services.AddScoped<IGardenItemRepository, GardenItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
