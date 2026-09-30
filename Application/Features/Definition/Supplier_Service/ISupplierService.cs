using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.Supplier_Service
{
    /// <summary>
    /// سرویس مدیریت تامین‌کنندگان.
    /// </summary>
    public interface ISupplierService : IGenericRepository<Supplier>
    {
        Task<IEnumerable<Supplier>> GetTopSuppliersAsync(int count);

        Task<Supplier?> GetByNameAsync(string name);

        Task<Supplier?> GetByEconomicCodeAsync(string economicCode);

        Task<IEnumerable<Supplier>> SearchAsync(string keyword);

        Task<bool> IsPhoneDuplicateAsync(string? phone, int? excludeSupplierId = null);

        /// <summary>تامین‌کنندگانی که کد اقتصادی دارند.</summary>
        Task<IEnumerable<Supplier>> GetWithEconomicCodeAsync();

        /// <summary>
        /// ویرایش تامین‌کننده با اعتبارسنجی تکراری نبودن شماره تلفن.
        /// توجه: برخلاف سایر سرویس‌ها این متد به‌جای tuple، exception پرتاب می‌کند.
        /// </summary>
        new Task<Supplier> UpdateAsync(Supplier entity);
    }
}
