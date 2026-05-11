using GardenApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardenApi.Infrastructure.Data.Configurations;

public class GardenConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
        builder.Property(g => g.Description).HasMaxLength(500);
        builder.Property(g => g.CreatedDate).IsRequired();
        builder.Property(g => g.ModifiedDate).IsRequired();

        builder.HasMany(g => g.GardenItems)
               .WithOne(gi => gi.Garden)
               .HasForeignKey(gi => gi.GardenId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
