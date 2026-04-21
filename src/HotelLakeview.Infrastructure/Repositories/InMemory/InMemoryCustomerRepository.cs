using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using System.Reflection;

namespace HotelLakeview.Infrastructure.Repositories;

/// <summary>
/// In-memory-toteutus asiakkaiden tallennukseen ja hakemiseen.
/// Tarkoitettu kehitys- ja testausvaiheen väliaikaiseksi toteutukseksi.
/// </summary>
public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private static readonly List<Customer> Customers = new();
    private static int _nextId = 1;
    private static bool _isSeeded = false;

    public InMemoryCustomerRepository()
    {
        SeedCustomers();
    }

    public Task<Customer?> GetByIdAsync(int id)
    {
        var customer = Customers.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByEmailAsync(string email)
    {
        var customer = Customers.FirstOrDefault(c =>
            !string.IsNullOrWhiteSpace(c.Email) &&
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(customer);
    }
        public Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
    {
        var customer = Customers.FirstOrDefault(c =>
            !string.IsNullOrWhiteSpace(c.PhoneNumber) &&
            c.PhoneNumber.Equals(phoneNumber, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(customer);
    }

    public Task<IReadOnlyCollection<Customer>> GetPagedAsync(int page, int pageSize)
    {
        var items = Customers
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Customer>)items);
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(Customers.Count);
    }

    public Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, PaginationRequest pagination)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            var allCustomers = Customers
                .OrderBy(c => c.Id)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList()
                .AsReadOnly();

            return Task.FromResult((IReadOnlyList<Customer>)allCustomers);
        }

        var normalized = searchTerm.Trim();

        var results = Customers
            .Where(c =>
                c.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(c.Email) &&
                 c.Email.Contains(normalized, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(c.PhoneNumber) &&
                 c.PhoneNumber.Contains(normalized, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyList<Customer>)results);
    }

    public Task<int> CountSearchResultsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(Customers.Count);
        }

        var normalized = searchTerm.Trim();

        var count = Customers.Count(c =>
            c.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrWhiteSpace(c.Email) &&
             c.Email.Contains(normalized, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(c.PhoneNumber) &&
             c.PhoneNumber.Contains(normalized, StringComparison.OrdinalIgnoreCase)));

        return Task.FromResult(count);
    }

    public Task<Customer> AddAsync(Customer customer)
    {
        SetEntityId(customer, _nextId++);
        Customers.Add(customer);

        return Task.FromResult(customer);
    }

    public Task<Customer> UpdateAsync(Customer customer)
    {
        var existingIndex = Customers.FindIndex(c => c.Id == customer.Id);

        if (existingIndex >= 0)
        {
            Customers[existingIndex] = customer;
        }

        return Task.FromResult(customer);
    }

    public Task DeleteAsync(int id)
    {
        var existingIndex = Customers.FindIndex(c => c.Id == id);

        if (existingIndex >= 0)
        {
            Customers.RemoveAt(existingIndex);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Lisää muistivarastoon oletusasiakkaat vain kerran.
    /// </summary>
    private static void SeedCustomers()
    {
        if (_isSeeded)
        {
            return;
        }

        var seedCustomers = new List<Customer>
        {
            new Customer("Liisa Järvinen", "liisa.jarvinen@example.com", "0401000001", "Pähkinäallergia"),
            new Customer("Matti Meikäläinen", "matti.meikalainen@example.com", "0401000002", "Myöhäinen saapuminen"),
            new Customer("Anna Virtanen", "anna.virtanen@example.com", "0401000003", "Hiljainen huone"),
            new Customer("Pekka Nieminen", "pekka.nieminen@example.com", "0401000004", "Lisätyyny"),
            new Customer("Sari Lahtinen", "sari.lahtinen@example.com", "0401000005", "Gluteeniton aamiainen"),
            new Customer("Janne Korhonen", "janne.korhonen@example.com", "0401000006", "Ei erityistoiveita"),
            new Customer("Emilia Heikkinen", "emilia.heikkinen@example.com", "0401000007", "Aikainen check-in"),
            new Customer("Oskari Lehtonen", "oskari.lehtonen@example.com", "0401000008", "Yläkerros toivottu"),
            new Customer("Laura Mäkinen", "laura.makinen@example.com", "0401000009", "Vauvasänky tarvitaan"),
            new Customer("Tomi Salonen", "tomi.salonen@example.com", "0401000010", "Lemmikki mukana")
        };

        foreach (var customer in seedCustomers)
        {
            SetEntityId(customer, _nextId++);
            Customers.Add(customer);
        }

        _isSeeded = true;
    }

    /// <summary>
    /// Asettaa entiteetin Id-arvon reflektiolla, koska setter on private.
    /// </summary>
    private static void SetEntityId(Customer customer, int id)
    {
        var property = typeof(Customer).GetProperty(
            nameof(Customer.Id),
            BindingFlags.Instance | BindingFlags.Public);

        property?.SetValue(customer, id);
    }
}