using Domain.Entity;

namespace Application.Features.Definition.Warehouse_Service
{
    /// <summary>
    /// سرویس مدیریت انبارها.
    /// توجه: این سرویس برخلاف بقیه از IGenericRepository ارث‌بری نمی‌کند
    /// و متدهای CRUD را مستقیماً تعریف می‌کند.
    /// </summary>
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAllAsync();

        Task<Warehouse> GetByIdAsync(int id);

        Task<Warehouse> AddAsync(Warehouse warehouse);

        Task<Warehouse> UpdateAsync(Warehouse warehouse);

        Task DeleteAsync(int id);

        Task<IEnumerable<Warehouse>> GetTopWarehousesAsync(int count);

        Task<Warehouse> GetByNameAsync(string name);

        /// <summary>انبارهایی که موجودی آن‌ها به آستانه داده‌شده رسیده یا از آن کمتر است.</summary>
        Task<IEnumerable<Warehouse>> GetLowCapacityWarehousesAsync(int threshold = 10);

        Task<IEnumerable<Warehouse>> GetByStatusAsync(string status);

        Task<IEnumerable<Warehouse>> SearchAsync(string keyword);

        Task<IEnumerable<Warehouse>> GetTopAsync(int count);

        Task<bool> IsWarehouseNameUniqueAsync(string name, int? excludeId = null);

        Task<IEnumerable<Warehouse>> SearchWarehousesAsync(string keyword);
    }
}
