using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.Customer_Service
{
    /// <summary>
    /// سرویس مدیریت مشتریان.
    /// </summary>
    public interface ICustomerService : IGenericRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetTopCustomersAsync(int count);

        Task<IEnumerable<Customer>> SearchByNameAsync(string name);

        Task<Customer?> GetByPhoneAsync(string phone);

        Task<bool> IsNameExistsAsync(string name, int? excludeId = null);

        Task<(bool Success, string Message, Customer? Customer)> CreateCustomerAsync(Customer customer);

        Task<(bool Success, string Message, Customer? Customer)> UpdateCustomerAsync(Customer customer);

        Task<(bool Success, string Message)> DeleteCustomerAsync(int id);

        Task<IEnumerable<Customer>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
