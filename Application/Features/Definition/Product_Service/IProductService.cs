using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.Product_Service
{
    /// <summary>
    /// سرویس مدیریت کالاها. علاوه بر عملیات عمومی، منطق کسب‌وکار و اعتبارسنجی کالا در این سرویس قرار دارد.
    /// </summary>
    public interface IProductService : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetTopProductsAsync(int count);

        Task<Product?> GetProductByIdAsync(Guid productId);

        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<IEnumerable<Product>> SearchByNameAsync(string name);

        Task<Product?> GetByBarcodeAsync(string barcode);

        Task<bool> IsBarcodeExistsAsync(string barcode, Guid? excludeId = null);

        Task<bool> IsProductNameExistsAsync(string name, Guid? excludeId = null);

        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>کالاهایی که موجودی آن‌ها به حداقل تعیین‌شده رسیده یا از آن کمتر است.</summary>
        Task<IEnumerable<Product>> GetLowStockProductsAsync();

        /// <summary>کالاهایی که تاریخ انقضای آن‌ها گذشته است.</summary>
        Task<IEnumerable<Product>> GetExpiredProductsAsync();

        Task<(bool Success, string Message, Product? Product)> CreateProductAsync(Product product);

        Task<(bool Success, string Message, Product? Product)> UpdateProductAsync(Product product);

        Task<(bool Success, string Message)> DeleteProductAsync(Guid productId);

        Task<(bool Success, string Message)> DeactivateProductAsync(Guid productId);

        Task<(bool Success, string Message)> ActivateProductAsync(Guid productId);

        Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
