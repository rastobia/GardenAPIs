using GardenApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardenApi.Infrastructure.Data.Configurations;

public class GardenItemPlantConfiguration : IEntityTypeConfiguration<GardenItemPlant>
{
    public void Configure(EntityTypeBuilder<GardenItemPlant> builder)
    {
        builder.HasKey(gip => gip.Id);
        builder.Property(gip => gip.AddedDate).IsRequired();

        builder.HasOne(gip => gip.GardenItem)
               .WithMany(gi => gi.GardenItemPlants)
               .HasForeignKey(gip => gip.GardenItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gip => gip.Plant)
               .WithMany(p => p.GardenItemPlants)
               .HasForeignKey(gip => gip.PlantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
