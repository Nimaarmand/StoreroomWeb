using Application.Features.Definition.Context;
using Application.Features.Definition.Category_Service;
using Application.Features.Definition.GenericRepository;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.Category_Service
{
    public class CategoryService : GenericRepository<Category>, ICategoryService
    {
        public CategoryService(IApplicationDbContext context) : base(context)
        {
        }

        // ========== Ù…ØªØ¯ Ø³ÙØ§Ø±Ø´ÛŒ Ø¨Ø±Ø§ÛŒ Ø¯Ø±ÛŒØ§ÙØª N Ø±Ú©ÙˆØ±Ø¯ Ø§ÙˆÙ„ ==========
        public async Task<IEnumerable<Category>> GetTopCategoryAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(c => c.CategoryId)
                .Take(count)
                .ToListAsync();
        }

        // ========== Ø³Ø§ÛŒØ± Ù…ØªØ¯Ù‡Ø§ÛŒ Ø³Ø±ÙˆÛŒØ³ (ØªÚ©â€ŒÙ†Ø³Ø®Ù‡) ==========
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _dbSet
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllCategoriesAsync();
            return await _dbSet
                .Where(c => c.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.CategoryId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> IsValidParentCategoryAsync(int? parentId, int? currentCategoryId = null)
        {
            if (!parentId.HasValue) return true;
            if (currentCategoryId.HasValue && parentId == currentCategoryId) return false;

            // Ù¾ÛŒÙ…ÙˆØ¯Ù† Ø²Ù†Ø¬ÛŒØ±Ù‡ ÙˆØ§Ù„Ø¯ ØªØ§ Ø±ÛŒØ´Ù‡. Ø¨Ø±Ø±Ø³ÛŒ ÙÙ‚Ø· ÙˆØ§Ù„Ø¯ Ù…Ø³ØªÙ‚ÛŒÙ… Ú©Ø§ÙÛŒ Ù†ÛŒØ³Øª:
            // A ÙˆØ§Ù„Ø¯Ù B Ùˆ B ÙˆØ§Ù„Ø¯Ù A ÛŒÚ© Ø­Ù„Ù‚Ù‡ Ù…ÛŒâ€ŒØ³Ø§Ø²Ø¯ Ú©Ù‡ Ù†Ù‡ Ø¯Ø±Ø®Øª Ø±Ø§ Ù…ÛŒâ€ŒØ´ÙˆØ¯ Ø±Ø³Ù… Ú©Ø±Ø¯
            // Ùˆ Ù†Ù‡ Ø¯Ø± Ø¨Ø§Ø²Ú¯Ø´Øª Ø§Ø² Ø±Ø§Ø¨Ø· Ú©Ø§Ø±Ø¨Ø±ÛŒ Ù‚Ø§Ø¨Ù„ Ú¯Ø±ÛŒØ² Ø§Ø³Øª.
            var visited = new HashSet<int>();
            var cursor = parentId;

            while (cursor.HasValue)
            {
                if (!visited.Add(cursor.Value)) return false;          // Ø­Ù„Ù‚Ù‡ Ø§Ø² Ù‚Ø¨Ù„ Ø¯Ø± Ø¯Ø§Ø¯Ù‡ ÙˆØ¬ÙˆØ¯ Ø¯Ø§Ø±Ø¯
                if (currentCategoryId.HasValue && cursor == currentCategoryId) return false;

                var node = await GetByIdAsync(cursor.Value);
                if (node == null) return false;
                cursor = node.ParentId;
            }

            return true;
        }

        public async Task<(bool Success, string Message, Category? Category)> CreateCategoryAsync(Category category)
        {
            if (category == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(category.Name)) return (false, "Ù†Ø§Ù… Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsCategoryNameExistsAsync(category.Name)) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);
            if (!await IsValidParentCategoryAsync(category.ParentId)) return (false, "ÙˆØ§Ù„Ø¯ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);

            category.CreatedAt = DateTime.Now;
            category.IsActive = true;

            var created = await AddAsync(category); // AddAsync Ù‚ÙÙ„ Ø¯Ø§Ø±Ø¯
            return (true, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø§Ø¶Ø§ÙÙ‡ Ø´Ø¯", created);
        }

        public async Task<(bool Success, string Message, Category? Category)> UpdateCategoryAsync(Category category)
        {
            if (category == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª", null);
            if (string.IsNullOrWhiteSpace(category.Name)) return (false, "Ù†Ø§Ù… Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ù†Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ø®Ø§Ù„ÛŒ Ø¨Ø§Ø´Ø¯", null);
            if (await IsCategoryNameExistsAsync(category.Name, category.CategoryId))
                return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø§ Ø§ÛŒÙ† Ù†Ø§Ù… Ù‚Ø¨Ù„Ø§Ù‹ Ø«Ø¨Øª Ø´Ø¯Ù‡ Ø§Ø³Øª", null);
            if (!await IsValidParentCategoryAsync(category.ParentId, category.CategoryId))
                return (false, "ÙˆØ§Ù„Ø¯ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡ Ù†Ø§Ù…Ø¹ØªØ¨Ø± Ø§Ø³Øª (Ø§Ø­ØªÙ…Ø§Ù„Ø§Ù‹ Ø®ÙˆØ¯ Ø¯Ø³ØªÙ‡ ÛŒØ§ Ù†ÙˆØ§Ø¯Ú¯Ø§Ù† Ø¢Ù† Ø§Ø³Øª)", null);

            // AsNoTracking ØªØ§ Ù†Ù…ÙˆÙ†Ù‡ Ù…ÙˆØ¬ÙˆØ¯ tracked Ù†Ø´ÙˆØ¯Ø› Ø¯Ø± ØºÛŒØ± Ø§ÛŒÙ† ØµÙˆØ±Øª
            // UpdateAsync Ø¨Ø§ Ø®Ø·Ø§ÛŒ Â«Ø¯Ùˆ Ù†Ù…ÙˆÙ†Ù‡ Ø¨Ø§ Ù‡Ù…Ø§Ù† Ú©Ù„ÛŒØ¯Â» Ø´Ú©Ø³Øª Ù…ÛŒâ€ŒØ®ÙˆØ±Ø¯.
            var existing = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
            if (existing == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯", null);

            category.CreatedAt = existing.CreatedAt;
            category.IsActive = existing.IsActive;

            var updated = await UpdateAsync(category); // UpdateAsync Ù‚ÙÙ„ Ø¯Ø§Ø±Ø¯
            return (true, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø¨Ø±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ø´Ø¯", updated);
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id); // GetCategoryByIdAsync Ù‚ÙÙ„ Ø¯Ø§Ø±Ø¯
            if (category == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯");

            // Ø¨Ø±Ø±Ø³ÛŒ Ø²ÛŒØ±Ø¯Ø³ØªÙ‡â€ŒÙ‡Ø§ Ù†ÛŒØ§Ø² Ø¨Ù‡ Ø¯Ø³ØªØ±Ø³ÛŒ Ø¨Ù‡ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ Ø¯Ø§Ø±Ø¯ØŒ Ø¨Ù†Ø§Ø¨Ø±Ø§ÛŒÙ† Ø¨Ø§ÛŒØ¯ Ù‚ÙÙ„ Ø¨Ú¯ÛŒØ±ÛŒÙ…
            var hasSub = await _dbSet.AnyAsync(c => c.ParentId == id);
            if (hasSub)
                return (false, "Ø§ÛŒÙ† Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¯Ø§Ø±Ø§ÛŒ Ø²ÛŒØ±Ø¯Ø³ØªÙ‡ Ø§Ø³Øª. Ø§Ø¨ØªØ¯Ø§ Ø²ÛŒØ±Ø¯Ø³ØªÙ‡â€ŒÙ‡Ø§ Ø±Ø§ Ø­Ø°Ù ÛŒØ§ Ø¬Ø§Ø¨Ù‡â€ŒØ¬Ø§ Ú©Ù†ÛŒØ¯.");
            // Ú©Ø§Ù„Ø§Ù‡Ø§ÛŒ Ø§ÛŒÙ† Ø¯Ø³ØªÙ‡ Ø¨Ø§ Ø­Ø°Ù Ø¢Ù† ÛŒØªÛŒÙ… Ù…ÛŒâ€ŒØ´ÙˆÙ†Ø¯Ø› Ø±Ø§Ø¨Ø·Ù‡ NO ACTION
            // Ø§Ø³Øª Ùˆ Ø¨Ø¯ÙˆÙ† Ø§ÛŒÙ† Ø¨Ø±Ø±Ø³ÛŒ Ø®Ø·Ø§ÛŒ Ú©Ù„ÛŒØ¯ Ø®Ø§Ø±Ø¬ÛŒ Ø¨Ù‡ Ú©Ø§Ø±Ø¨Ø± Ù†Ø´Ø§Ù† Ø¯Ø§Ø¯Ù‡ Ù…ÛŒâ€ŒØ´ÙˆØ¯.
            var productCount = await _context.Set<Product>().CountAsync(p => p.CategoryId == id);
            if (productCount > 0)
                return (false, $"Ø§ÛŒÙ† Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ {productCount} Ú©Ø§Ù„Ø§ Ø¯Ø§Ø±Ø¯ Ùˆ Ù‚Ø§Ø¨Ù„ Ø­Ø°Ù Ù†ÛŒØ³Øª. Ø§Ø¨ØªØ¯Ø§ Ú©Ø§Ù„Ø§Ù‡Ø§ Ø±Ø§ Ø¨Ù‡ Ø¯Ø³ØªÙ‡ Ø¯ÛŒÚ¯Ø±ÛŒ Ù…Ù†ØªÙ‚Ù„ Ú©Ù†ÛŒØ¯ ÛŒØ§ Ø¯Ø³ØªÙ‡ Ø±Ø§ ØºÛŒØ±ÙØ¹Ø§Ù„ Ú©Ù†ÛŒØ¯.");
            await RemoveAsync(category); // RemoveAsync Ù‚ÙÙ„ Ø¯Ø§Ø±Ø¯
            return (true, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø§ Ù…ÙˆÙÙ‚ÛŒØª Ø­Ø°Ù Ø´Ø¯");
        }

        public async Task<(bool Success, string Message)> DeactivateCategoryAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯");
            category.IsActive = false;
            await UpdateAsync(category);
            return (true, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ØºÛŒØ±ÙØ¹Ø§Ù„ Ø´Ø¯");
        }

        public async Task<(bool Success, string Message)> ActivateCategoryAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return (false, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯");
            category.IsActive = true;
            await UpdateAsync(category);
            return (true, "Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ ÙØ¹Ø§Ù„ Ø´Ø¯");
        }

        public async Task<IEnumerable<Category>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
