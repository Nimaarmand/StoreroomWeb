using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.Category_Service
{
    /// <summary>
    /// سرویس مدیریت دسته‌بندی‌ها (با ساختار درختی والد/فرزند).
    /// </summary>
    public interface ICategoryService : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetTopCategoryAsync(int count);

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(int id);

        /// <summary>دسته‌بندی‌های سطح بالا (بدون والد).</summary>
        Task<IEnumerable<Category>> GetRootCategoriesAsync();

        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);

        Task<IEnumerable<Category>> SearchByNameAsync(string name);

        Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);

        /// <summary>بررسی اینکه والد انتخاب‌شده معتبر است و باعث حلقه در درخت نمی‌شود.</summary>
        Task<bool> IsValidParentCategoryAsync(int? parentId, int? currentCategoryId = null);

        Task<(bool Success, string Message, Category? Category)> CreateCategoryAsync(Category category);

        Task<(bool Success, string Message, Category? Category)> UpdateCategoryAsync(Category category);

        /// <summary>در صورتی که زیردسته داشته باشد، حذف انجام نمی‌شود.</summary>
        Task<(bool Success, string Message)> DeleteCategoryAsync(int id);

        Task<(bool Success, string Message)> DeactivateCategoryAsync(int id);

        Task<(bool Success, string Message)> ActivateCategoryAsync(int id);

        Task<IEnumerable<Category>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
