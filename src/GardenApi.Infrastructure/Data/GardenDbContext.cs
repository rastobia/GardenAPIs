using GardenApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Infrastructure.Data;

public class GardenDbContext : DbContext
{
    public GardenDbContext(DbContextOptions<GardenDbContext> options) : base(options) { }

    public DbSet<Garden> Gardens => Set<Garden>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<GardenItem> GardenItems => Set<GardenItem>();
    public DbSet<GardenItemPlant> GardenItemPlants => Set<GardenItemPlant>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GardenDbContext).Assembly);
    }
}
