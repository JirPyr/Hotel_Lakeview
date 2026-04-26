using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories.Ef;

/// <summary>
/// EF Core -toteutus asiakkaiden hallintaan.
/// </summary>
public sealed class CustomerRepository : ICustomerRepository
{
    private readonly HotelLakeviewDbContext _dbContext;

    public CustomerRepository(HotelLakeviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(c =>
                c.Email != null &&
                c.Email.ToLower() == email.ToLower());
    }

    public async Task<IReadOnlyCollection<Customer>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbContext.Customers
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(c =>
                c.PhoneNumber != null &&
                c.PhoneNumber == phoneNumber);
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Customers.CountAsync();
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, PaginationRequest pagination)
    {
        IQueryable<Customer> query = _dbContext.Customers;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToLower();

            query = query.Where(c =>
                c.FullName.ToLower().Contains(normalized) ||
                (c.Email != null && c.Email.ToLower().Contains(normalized)) ||
                (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(normalized)));
        }

        return await query
            .OrderBy(c => c.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchResultsAsync(string searchTerm)
    {
        IQueryable<Customer> query = _dbContext.Customers;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToLower();

            query = query.Where(c =>
                c.FullName.ToLower().Contains(normalized) ||
                (c.Email != null && c.Email.ToLower().Contains(normalized)) ||
                (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(normalized)));
        }

        return await query.CountAsync();
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _dbContext.Customers.Update(customer);
        await _dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return;
        }

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();
    }
}