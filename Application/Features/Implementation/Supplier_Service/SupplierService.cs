using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.Supplier_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.Supplier_Service
{
    public class SupplierService : GenericRepository<Supplier>, ISupplierService
    {
        public SupplierService(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetTopSuppliersAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(p => p.SupplierId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return await _dbSet.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Supplier?> GetByEconomicCodeAsync(string economicCode)
        {
            if (string.IsNullOrWhiteSpace(economicCode)) return null;
            return await _dbSet.FirstOrDefaultAsync(s => s.EconomicCode == economicCode);
        }

        public async Task<IEnumerable<Supplier>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync(); 

            keyword = keyword.Trim();
            return await _dbSet
                .Where(s => s.Name.Contains(keyword) ||
                            (s.Phone != null && s.Phone.Contains(keyword)) ||
                            (s.Address != null && s.Address.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<bool> IsPhoneDuplicateAsync(string? phone, int? excludeSupplierId = null)
        {
            // Ø¨Ø¯ÙˆÙ† ØªÙ„ÙÙ† ØªÚ©Ø±Ø§Ø±ÛŒ Ù…Ø¹Ù†Ø§ Ù†Ø¯Ø§Ø±Ø¯Ø› Ù‡Ù…Ú†Ù†ÛŒÙ† Ù…Ù‚Ø§ÛŒØ³Ù‡ Ø¨Ø§ null Ø¯Ø± SQL
            // ØªØ£Ù…ÛŒÙ†â€ŒÚ©Ù†Ù†Ø¯Ù‡â€ŒÙ‡Ø§ÛŒ Ø¨Ø¯ÙˆÙ† ØªÙ„ÙÙ† Ø±Ø§ ØªÚ©Ø±Ø§Ø±ÛŒ Ø¬Ù„ÙˆÙ‡ Ù…ÛŒâ€ŒØ¯Ø§Ø¯.
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var query = _dbSet.Where(s => s.Phone == phone);
            if (excludeSupplierId.HasValue)
                query = query.Where(s => s.SupplierId != excludeSupplierId.Value);
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Supplier>> GetWithEconomicCodeAsync()
        {
            return await _dbSet.Where(s => !string.IsNullOrWhiteSpace(s.EconomicCode)).ToListAsync();
        }

        public override async Task<Supplier> UpdateAsync(Supplier entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

          
            if (await IsPhoneDuplicateAsync(entity.Phone, entity.SupplierId))
                throw new InvalidOperationException("Ø´Ù…Ø§Ø±Ù‡ ØªÙ„ÙÙ† ØªÚ©Ø±Ø§Ø±ÛŒ Ø§Ø³Øª.");
            return await base.UpdateAsync(entity); 
        }
       
    }
}
