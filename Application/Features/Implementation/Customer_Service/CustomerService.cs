using Application.Features.Definition.Context;
using Application.Features.Definition.Customer_Service;
using Application.Features.Definition.GenericRepository;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.Customer_Service
{
    public class CustomerService : GenericRepository<Customer>, ICustomerService
    {
        public CustomerService(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(p => p.CustomerId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllAsync();
            return await _dbSet.Where(c => c.Name.Contains(name)).ToListAsync();
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            return await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.CustomerId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<(bool Success, string Message, Customer? Customer)> CreateCustomerAsync(Customer customer)
        {
            if (customer == null) return (false, "Ù…Ø´ØªØ±ÛŒ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(customer.Name)) return (false, "Ù†Ø§Ù… Ù…Ø´ØªØ±ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsNameExistsAsync(customer.Name)) return (false, "Ù…Ø´ØªØ±ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);

            customer.Creationdate = DateTime.Now;
            var created = await AddAsync(customer);
            return (true, "Ù…Ø´ØªØ±ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø§Ø¶Ø§ÙÙ‡ Ø´Ø¯", created);
        }

        public async Task<(bool Success, string Message, Customer? Customer)> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null) return (false, "Ù…Ø´ØªØ±ÛŒ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(customer.Name)) return (false, "Ù†Ø§Ù… Ù…Ø´ØªØ±ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsNameExistsAsync(customer.Name, customer.CustomerId))
                return (false, "Ù…Ø´ØªØ±ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);

            // AsNoTracking ØªØ§ Ù†Ù…ÙˆÙ†Ù‡ Ù…ÙˆØ¬ÙˆØ¯ tracked Ù†Ø´ÙˆØ¯Ø› Ø¯Ø± ØºÛŒØ± Ø§ÛŒÙ† ØµÙˆØ±Øª
            // UpdateAsync Ø¨Ø§ Ø®Ø·Ø§ÛŒ Â«Ø¯Ùˆ Ù†Ù…ÙˆÙ†Ù‡ Ø¨Ø§ Ù‡Ù…Ø§Ù† Ú©Ù„ÛŒØ¯Â» Ø´Ú©Ø³Øª Ù…ÛŒâ€ŒØ®ÙˆØ±Ø¯.
            var existing = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
            if (existing == null) return (false, "Ù…Ø´ØªØ±ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯", null);

            customer.Creationdate = existing.Creationdate;
            var updated = await UpdateAsync(customer);
            return (true, "Ù…Ø´ØªØ±ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø¨Ø±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ø´Ø¯", updated);
        }

        public async Task<(bool Success, string Message)> DeleteCustomerAsync(int id)
        {
            var customer = await GetByIdAsync(id);
            if (customer == null) return (false, "Ù…Ø´ØªØ±ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯");

            // Ø±Ø§Ø¨Ø·Ù‡ Ø¨Ù‡â€ŒØµÙˆØ±Øª Ø§Ø®ØªÛŒØ§Ø±ÛŒ ØªØ¹Ø±ÛŒÙ Ø´Ø¯Ù‡ Ùˆ Ø¯Ø± Ø¯ÛŒØªØ§Ø¨ÛŒØ³ NO ACTION Ø§Ø³ØªØŒ Ù¾Ø³
            // Ø­Ø°Ù Ù…Ø´ØªØ±ÛŒÙ Ø¯Ø§Ø±Ø§ÛŒ Ø­ÙˆØ§Ù„Ù‡ Ø¨Ù‡ Ø®Ø·Ø§ÛŒ Ø®Ø§Ù… Ú©Ù„ÛŒØ¯ Ø®Ø§Ø±Ø¬ÛŒ Ø®ØªÙ… Ù…ÛŒâ€ŒØ´ÙˆØ¯.
            // Ù¾ÛŒØ´ Ø§Ø² Ø±Ø³ÛŒØ¯Ù† Ø¨Ù‡ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ Ø¨Ø±Ø±Ø³ÛŒ Ù…ÛŒâ€ŒÚ©Ù†ÛŒÙ… ØªØ§ Ù¾ÛŒØ§Ù… Ù‚Ø§Ø¨Ù„ ÙÙ‡Ù… Ø¨Ø¯Ù‡ÛŒÙ….
            var issueCount = await _context.Set<GoodsIssue>()
                .CountAsync(i => i.CustomerId == id);

            if (issueCount > 0)
                return (false, $"Ø§ÛŒÙ† Ù…Ø´ØªØ±ÛŒ {issueCount} Ø­ÙˆØ§Ù„Ù‡ Ø¯Ø§Ø±Ø¯ Ùˆ Ù‚Ø§Ø¨Ù„ Ø­Ø°Ù Ù†ÛŒØ³Øª. Ø­ÙˆØ§Ù„Ù‡â€ŒÙ‡Ø§ Ø±Ø§ Ø§Ø¨ØªØ¯Ø§ Ø¨Ø±Ø±Ø³ÛŒ Ú©Ù†ÛŒØ¯.");

            await RemoveAsync(customer);
            return (true, "Ù…Ø´ØªØ±ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø­Ø°Ù Ø´Ø¯");
        }

        public async Task<IEnumerable<Customer>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
