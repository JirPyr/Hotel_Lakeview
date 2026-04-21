using HotelLakeview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelLakeview.Infrastructure.Persistence.Configurations;

/// <summary>
/// Huonekuvan EF Core -konfiguraatio.
/// </summary>
public sealed class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
{
    public void Configure(EntityTypeBuilder<RoomImage> builder)
    {
        builder.ToTable("room_images");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoomId)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.FilePathOrUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .IsRequired();
    }
}