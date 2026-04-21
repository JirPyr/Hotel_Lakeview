using HotelLakeview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelLakeview.Infrastructure.Persistence.Configurations;

/// <summary>
/// Huone-entiteetin EF Core -konfiguraatio.
/// </summary>
public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoomNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.RoomType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.MaxGuests)
            .IsRequired();

        builder.Property(x => x.BasePricePerNight)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.RoomNumber)
            .IsUnique();

        builder.HasMany<RoomImage>()
            .WithOne()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}