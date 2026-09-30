using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.Product_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.Product_Service
{
    public class ProductService : GenericRepository<Product>, IProductService
    {
        public ProductService(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetTopProductsAsync(int count)
        {
            return await _dbSet
                .Include(p=>p.Category)
                .OrderByDescending(p => p.ProductId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopProductsOrderedAsync<TKey>(
            int count,
            Expression<Func<Product, TKey>> orderBy,
            bool ascending = true)
        {
            IQueryable<Product> query = _dbSet;
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.Take(count).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllProductsAsync();
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<bool> IsBarcodeExistsAsync(string barcode, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return false;
            var query = _dbSet.Where(p => p.Barcode == barcode);
            if (excludeId.HasValue)
                query = query.Where(p => p.ProductId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> IsProductNameExistsAsync(string name, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Name == name);
            if (excludeId.HasValue)
                query = query.Where(p => p.ProductId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.MinStockLevel.HasValue && p.Number < p.MinStockLevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetExpiredProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.ExpirationDate < DateTime.Now)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, Product? Product)> CreateProductAsync(Product product)
        {
            if (product == null) return (false, "Ù…Ø­ØµÙˆÙ„ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(product.Name)) return (false, "Ù†Ø§Ù… Ù…Ø­ØµÙˆÙ„ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (string.IsNullOrWhiteSpace(product.BaseUnit)) return (false, "ÙˆØ§Ø­Ø¯ Ù¾Ø§ÛŒÙ‡ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsProductNameExistsAsync(product.Name)) return (false, "Ù…Ø­ØµÙˆÙ„ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);
            if (!string.IsNullOrWhiteSpace(product.Barcode) && await IsBarcodeExistsAsync(product.Barcode))
                return (false, "Ø¨Ø§Ø±Ú©Ø¯ ØªÚ©Ø±Ø§Ø±ÛŒ Ø§Ø³Øª", null);
            if (product.Number < 0) return (false, "ØªØ¹Ø¯Ø§Ø¯ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ù…Ù†ÙÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (product.MinStockLevel.HasValue && product.MaxStockLevel.HasValue && product.MinStockLevel > product.MaxStockLevel)
                return (false, "Ø­Ø¯Ø§Ù‚Ù„ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø§Ø² Ø­Ø¯Ø§Ú©Ø«Ø± Ø¨ÛŒØ´ØªØ± Ø¨Ø§Ø´Ø¯", null);
            if (product.ProductionDate.HasValue && product.ExpirationDate.HasValue && product.ProductionDate > product.ExpirationDate)
                return (false, "ØªØ§Ø±ÛŒØ® ØªÙˆÙ„ÛŒØ¯ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø¨Ø¹Ø¯ Ø§Ø² ØªØ§Ø±ÛŒØ® Ø§Ù†Ù‚Ø¶Ø§ Ø¨Ø§Ø´Ø¯", null);
            if (product.Weight < 0) return (false, "ÙˆØ²Ù† Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ù…Ù†ÙÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (product.CategoryId.HasValue)
            {
                var categoryExists = await _context.Set<Category>().AnyAsync(c => c.CategoryId == product.CategoryId);
                if (!categoryExists) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø§Ù†ØªØ®Ø§Ø¨ÛŒ ÙˆØ¬ÙˆØ¯ Ù†Ø¯Ø§Ø±Ø¯", null);
            }

            product.ProductId = Guid.NewGuid();
            product.CreatedAt = DateTime.Now;
            product.IsActive = true;
            var created = await AddAsync(product);
            return (true, "Ù…Ø­ØµÙˆÙ„ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø§Ø¶Ø§ÙÙ‡ Ø´Ø¯", created);
        }

        public async Task<(bool Success, string Message, Product? Product)> UpdateProductAsync(Product product)
        {
            if (product == null) return (false, "Ù…Ø­ØµÙˆÙ„ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(product.Name)) return (false, "Ù†Ø§Ù… Ù…Ø­ØµÙˆÙ„ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (string.IsNullOrWhiteSpace(product.BaseUnit)) return (false, "ÙˆØ§Ø­Ø¯ Ù¾Ø§ÛŒÙ‡ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsProductNameExistsAsync(product.Name, product.ProductId))
                return (false, "Ù…Ø­ØµÙˆÙ„ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);
            if (!string.IsNullOrWhiteSpace(product.Barcode) && await IsBarcodeExistsAsync(product.Barcode, product.ProductId))
                return (false, "Ø¨Ø§Ø±Ú©Ø¯ ØªÚ©Ø±Ø§Ø±ÛŒ Ø§Ø³Øª", null);
            if (product.Number < 0) return (false, "ØªØ¹Ø¯Ø§Ø¯ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ù…Ù†ÙÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (product.MinStockLevel.HasValue && product.MaxStockLevel.HasValue && product.MinStockLevel > product.MaxStockLevel)
                return (false, "Ø­Ø¯Ø§Ù‚Ù„ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø§Ø² Ø­Ø¯Ø§Ú©Ø«Ø± Ø¨ÛŒØ´ØªØ± Ø¨Ø§Ø´Ø¯", null);
            if (product.ProductionDate.HasValue && product.ExpirationDate.HasValue && product.ProductionDate > product.ExpirationDate)
                return (false, "ØªØ§Ø±ÛŒØ® ØªÙˆÙ„ÛŒØ¯ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø¨Ø¹Ø¯ Ø§Ø² ØªØ§Ø±ÛŒØ® Ø§Ù†Ù‚Ø¶Ø§ Ø¨Ø§Ø´Ø¯", null);
            if (product.Weight < 0) return (false, "ÙˆØ²Ù† Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ù…Ù†ÙÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (product.CategoryId.HasValue)
            {
                var categoryExists = await _context.Set<Category>().AnyAsync(c => c.CategoryId == product.CategoryId);
                if (!categoryExists) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø§Ù†ØªØ®Ø§Ø¨ÛŒ ÙˆØ¬ÙˆØ¯ Ù†Ø¯Ø§Ø±Ø¯", null);
            }

            // AsNoTracking Ù„Ø§Ø²Ù… Ø§Ø³Øª: Ø§Ú¯Ø± Ø±Ú©ÙˆØ±Ø¯ Ù…ÙˆØ¬ÙˆØ¯ Ø±Ø§ tracked Ø¨Ø§Ø±Ú¯Ø°Ø§Ø±ÛŒ
            // Ú©Ù†ÛŒÙ… Ùˆ Ø¨Ø¹Ø¯ Ù†Ù…ÙˆÙ†Ù‡ ÙˆØ±ÙˆØ¯ÛŒÙ Ø¬Ø¯Ø§Ú¯Ø§Ù†Ù‡ Ø±Ø§ Ø¨Ù‡ Update Ø¨Ø¯Ù‡ÛŒÙ…ØŒ EF Ø¨Ø§ Ø®Ø·Ø§ÛŒ
            // Â«Ø¯Ùˆ Ù†Ù…ÙˆÙ†Ù‡ Ø¨Ø§ Ù‡Ù…Ø§Ù† Ú©Ù„ÛŒØ¯Â» Ø´Ú©Ø³Øª Ù…ÛŒâ€ŒØ®ÙˆØ±Ø¯. Ø§ÛŒÙ†Ø¬Ø§ ÙÙ‚Ø· ÙˆØ¬ÙˆØ¯ Ø±Ú©ÙˆØ±Ø¯
            // Ø±Ø§ Ú†Ú© Ù…ÛŒâ€ŒÚ©Ù†ÛŒÙ… ØªØ§ UpdateAsync Ø¨ØªÙˆØ§Ù†Ø¯ Ù†Ù…ÙˆÙ†Ù‡ ÙˆØ±ÙˆØ¯ÛŒ Ø±Ø§ attach Ú©Ù†Ø¯.
            var existing = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            if (existing == null) return (false, "Ù…Ø­ØµÙˆÙ„ ÛŒØ§ÙØª Ù†Ø´Ø¯", null);

            product.CreatedAt = existing.CreatedAt;
            product.IsActive = existing.IsActive;
            var updated = await UpdateAsync(product);
            return (true, "Ù…Ø­ØµÙˆÙ„ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø¨Ø±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ø´Ø¯", updated);
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(Guid productId)
        {
            var product = await GetByIdAsync(productId);
            if (product == null) return (false, "Ù…Ø­ØµÙˆÙ„ ÛŒØ§ÙØª Ù†Ø´Ø¯");

            // Ù…Ø­ØµÙˆÙ„ Ø¯Ø§Ø±Ø§ÛŒ Ø³Ø§Ø¨Ù‚Ù‡ ØªØ±Ø§Ú©Ù†Ø´ Ù†Ø¨Ø§ÛŒØ¯ Ø­Ø°Ù Ø´ÙˆØ¯Ø› ØªØ§Ø±ÛŒØ®Ú†Ù‡ Ø§Ù†Ø¨Ø§Ø± Ø¨Ø§ÛŒØ¯
            // Ù‚Ø§Ø¨Ù„ Ø­Ø³Ø§Ø¨Ø±Ø³ÛŒ Ø¨Ù…Ø§Ù†Ø¯. Ø­Ø°Ù ÙÙ‚Ø· Ø¨Ø±Ø§ÛŒ Ù…Ø­ØµÙˆÙ„ Ø¨Ø¯ÙˆÙ† ØªØ±Ø§Ú©Ù†Ø´ Ù…Ø¬Ø§Ø² Ø§Ø³Øª.
            var receiptCount = await _context.Set<GoodsReceipt>().CountAsync(r => r.ProductId == productId);
            var issueCount = await _context.Set<GoodsIssue>().CountAsync(i => i.ProductId == productId);

            if (receiptCount > 0 || issueCount > 0)
                return (false, $"Ø§ÛŒÙ† Ù…Ø­ØµÙˆÙ„ {receiptCount} Ø±Ø³ÛŒØ¯ Ùˆ {issueCount} Ø­ÙˆØ§Ù„Ù‡ Ø¯Ø§Ø±Ø¯ Ùˆ Ø¨Ù‡ Ø¯Ù„ÛŒÙ„ Ø­ÙØ¸ Ø³ÙˆØ§Ø¨Ù‚ Ù…Ø§Ù„ÛŒ Ù‚Ø§Ø¨Ù„ Ø­Ø°Ù Ù†ÛŒØ³Øª. Ø¢Ù† Ø±Ø§ ØºÛŒØ±ÙØ¹Ø§Ù„ Ú©Ù†ÛŒØ¯.");

            await RemoveAsync(product);
            return (true, "Ù…Ø­ØµÙˆÙ„ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø­Ø°Ù Ø´Ø¯");
        }

        public async Task<(bool Success, string Message)> DeactivateProductAsync(Guid productId)
        {
            var product = await GetByIdAsync(productId);
            if (product == null) return (false, "Ù…Ø­ØµÙˆÙ„ ÛŒØ§ÙØª Ù†Ø´Ø¯");
            product.IsActive = false;
            await UpdateAsync(product);
            return (true, "Ù…Ø­ØµÙˆÙ„ ØºÛŒØ±ÙØ¹Ø§Ù„ Ø´Ø¯");
        }

        public async Task<(bool Success, string Message)> ActivateProductAsync(Guid productId)
        {
            var product = await GetByIdAsync(productId);
            if (product == null) return (false, "Ù…Ø­ØµÙˆÙ„ ÛŒØ§ÙØª Ù†Ø´Ø¯");
            product.IsActive = true;
            await UpdateAsync(product);
            return (true, "Ù…Ø­ØµÙˆÙ„ ÙØ¹Ø§Ù„ Ø´Ø¯");
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
