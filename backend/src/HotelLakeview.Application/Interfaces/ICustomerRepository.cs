using HotelLakeview.Application.Common.Models;
using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
    Task<IReadOnlyCollection<Customer>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, PaginationRequest pagination);
    Task<int> CountSearchResultsAsync(string searchTerm);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}