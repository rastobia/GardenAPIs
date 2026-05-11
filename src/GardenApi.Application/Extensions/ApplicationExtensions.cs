using GardenApi.Application.Interfaces;
using GardenApi.Application.Mappings;
using GardenApi.Application.Services;
using GardenApi.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GardenApi.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IGardenService, GardenService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IGardenItemService, GardenItemService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
