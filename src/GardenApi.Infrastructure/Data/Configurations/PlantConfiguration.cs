using GardenApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardenApi.Infrastructure.Data.Configurations;

public class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PlantingZone).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Spacing).HasColumnType("decimal(6,2)");
        builder.Property(p => p.Type).HasConversion<string>();
        builder.Property(p => p.SunlightRequirement).HasConversion<string>();
    }
}
