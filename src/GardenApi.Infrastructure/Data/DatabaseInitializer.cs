using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GardenApi.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GardenDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GardenDbContext>>();

        // Retry connecting — SQL Server container may need time to start
        for (var attempt = 1; attempt <= 12; attempt++)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                break;
            }
            catch (Exception ex) when (attempt < 12)
            {
                logger.LogWarning(
                    "Database not ready (attempt {Attempt}/12): {Message}. Retrying in 5s...",
                    attempt, ex.Message);
                await Task.Delay(5000);
            }
        }

        if (!await context.Plants.AnyAsync())
        {
            logger.LogInformation("Seeding plant data...");
            var sql = ReadSeedSql();
            await context.Database.ExecuteSqlRawAsync(sql);
            logger.LogInformation("Plant seed data applied successfully.");
        }
    }

    private static string ReadSeedSql()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith("seed_plants.sql"));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
