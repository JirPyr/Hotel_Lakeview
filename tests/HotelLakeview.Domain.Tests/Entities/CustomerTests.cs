using FluentAssertions;
using HotelLakeview.Domain.Entities;
using Xunit;

namespace HotelLakeview.Domain.Tests.Entities;

/// <summary>
/// Yksikkötestit Customer-entiteetille.
/// </summary>
public sealed class CustomerTests
{
    /// <summary>
    /// Varmistaa, että Deactivate asettaa asiakkaan inaktiiviseksi.
    /// </summary>
    [Fact]
    public void Deactivate_ShouldSetCustomerInactive()
    {
        // Arrange
        var customer = new Customer(
            fullName: "Test Asiakas",
            email: "test@example.com",
            phoneNumber: "0401234567",
            notes: "Testi");

        // Act
        customer.Deactivate();

        // Assert
        customer.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Varmistaa, että UpdateDetails päivittää asiakkaan tiedot oikein.
    /// </summary>
    [Fact]
    public void UpdateDetails_ShouldUpdateProperties()
    {
        // Arrange
        var customer = new Customer(
            fullName: "Test Asiakas",
            email: "test@example.com",
            phoneNumber: "0401234567",
            notes: "Vanha");

        // Act
        customer.UpdateDetails(
            fullName: "Päivitetty Asiakas",
            email: "uusi@example.com",
            phoneNumber: "0507654321",
            notes: "Uusi muistiinpano");

        // Assert
        customer.FullName.Should().Be("Päivitetty Asiakas");
        customer.Email.Should().Be("uusi@example.com");
        customer.PhoneNumber.Should().Be("0507654321");
        customer.Notes.Should().Be("Uusi muistiinpano");
    }
}