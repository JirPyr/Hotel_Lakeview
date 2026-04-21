using HotelLakeview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelLakeview.Infrastructure.Persistence.Configurations;

/// <summary>
/// Varausentiteetin EF Core -konfiguraatio.
/// </summary>
public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.RoomId)
            .IsRequired();

        builder.Property(x => x.CheckInDate)
            .IsRequired();

        builder.Property(x => x.CheckOutDate)
            .IsRequired();

        builder.Property(x => x.GuestCount)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CancelledAtUtc);

        builder.HasIndex(x => x.RoomId);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => new { x.RoomId, x.CheckInDate, x.CheckOutDate });

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}