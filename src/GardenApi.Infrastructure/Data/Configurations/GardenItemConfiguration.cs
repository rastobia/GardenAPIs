using GardenApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardenApi.Infrastructure.Data.Configurations;

public class GardenItemConfiguration : IEntityTypeConfiguration<GardenItem>
{
    public void Configure(EntityTypeBuilder<GardenItem> builder)
    {
        builder.HasKey(gi => gi.Id);
        builder.Property(gi => gi.Nickname).HasMaxLength(100);
        builder.Property(gi => gi.Height).HasColumnType("decimal(8,2)");
        builder.Property(gi => gi.Width).HasColumnType("decimal(8,2)");
        builder.Property(gi => gi.Type).HasConversion<string>();
        builder.Property(gi => gi.SunlightReceived).HasConversion<string>();
        builder.Property(gi => gi.CreatedDate).IsRequired();
        builder.Property(gi => gi.UpdatedDate).IsRequired();
    }
}
