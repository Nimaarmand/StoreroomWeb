using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.Warehouse_Service;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.Warehouse_Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IGenericRepository<Warehouse> _warehouseRepo;
        private readonly IApplicationDbContext _context;

        public WarehouseService(IGenericRepository<Warehouse> warehouseRepo, IApplicationDbContext context)
        {
            _warehouseRepo = warehouseRepo ?? throw new ArgumentNullException(nameof(warehouseRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ========== CRUD ==========
        public async Task<IEnumerable<Warehouse>> GetAllAsync()
            => await _warehouseRepo.GetAllAsync();

        public async Task<Warehouse?> GetByIdAsync(int id)
            => await _warehouseRepo.GetByIdAsync(id);

        public async Task<Warehouse> AddAsync(Warehouse warehouse)
            => await _warehouseRepo.AddAsync(warehouse);

        public async Task<Warehouse> UpdateAsync(Warehouse warehouse)
        {
            // موجودی انبار فقط با رسید و حواله تغییر می‌کند؛ مقدار ارسالی از
            // فرم ویرایش نباید آن را بازنویسی کند. این قانون دامنه است،
            // پس اینجا اعمال می‌شود نه در کنترلر.
            var current = await _warehouseRepo.GetByIdAsync(warehouse.WarehouseId);
            if (current == null) throw new InvalidOperationException("انبار یافت نشد");

            warehouse.Number = current.Number;
            return await _warehouseRepo.UpdateAsync(warehouse);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var warehouse = await GetByIdAsync(id);
            if (warehouse == null) return (false, "انبار یافت نشد");

            // انبار دارای تراکنش نباید حذف شود؛ بدون این بررسی خطای خام
            // کلید خارجی به کاربر نشان داده می‌شود.
            var receiptCount = await _context.Set<GoodsReceipt>().CountAsync(r => r.WarehouseId == id);
            var issueCount = await _context.Set<GoodsIssue>().CountAsync(i => i.WarehouseId == id);

            if (receiptCount > 0 || issueCount > 0)
                return (false, $"این انبار {receiptCount} رسید و {issueCount} حواله دارد و به دلیل حفظ سوابق مالی قابل حذف نیست.");

            await _warehouseRepo.RemoveByIdAsync(id);
            return (true, "انبار با موفقیت حذف شد");
        }

        public Task<IEnumerable<Warehouse>> GetTopWarehousesAsync(int count)
        {
            return _warehouseRepo.TakeAsync(count);
        }

        // ========== متدهای اختصاصی ==========
        public async Task<Warehouse?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var items = await _warehouseRepo.FindAsync(w => w.Name == name);
            return items.FirstOrDefault();
        }

        public async Task<IEnumerable<Warehouse>> GetLowCapacityWarehousesAsync(int threshold = 10)
        {
            var all = await _warehouseRepo.GetAllAsync();
            return all.Where(w => (w.Max - w.Min) < threshold);
        }

        public async Task<IEnumerable<Warehouse>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<Warehouse>();
            return await _warehouseRepo.FindAsync(w => w.Status == status);
        }

        public async Task<IEnumerable<Warehouse>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();
            keyword = keyword.Trim();
            return await _warehouseRepo.FindAsync(w =>
                w.Name.Contains(keyword) || (w.Location != null && w.Location.Contains(keyword)));
        }

        public async Task<IEnumerable<Warehouse>> GetTopAsync(int count)
        {
            var all = await _warehouseRepo.GetAllAsync();
            return all.Take(count);
        }

        public async Task<bool> IsWarehouseNameUniqueAsync(string name, int? excludeId = null)
        {
            var warehouses = await _warehouseRepo.FindAsync(w => w.Name == name);
            if (excludeId.HasValue)
                warehouses = warehouses.Where(w => w.WarehouseId != excludeId.Value);
            return !warehouses.Any();
        }

        // ========== متد جستجوی پیشرفته با پشتیبانی از Code و Location ==========
        public async Task<IEnumerable<Warehouse>> SearchWarehousesAsync(string keyword)
        {
          
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            keyword = keyword.Trim().ToLower();

            
            return await _warehouseRepo.FindAsync(w =>
                (w.Name != null && w.Name.ToLower().Contains(keyword)) ||
               
                (w.Location != null && w.Location.ToLower().Contains(keyword))
            );
        }
    }
}
