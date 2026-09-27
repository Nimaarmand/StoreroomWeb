using Application.Dto;
using Application.Features.Definition.Usermanagement_Service;
using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.GoodsReceipt_Service
{
    public class GoodsReceiptService : GenericRepository<GoodsReceipt>, IGoodsReceiptService
    {
        private readonly IUserManagement _usermanagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoodsReceiptService(IApplicationDbContext context,
            IDbLock dbLock,
            IUserManagement usermanagementService,
            UserManager<ApplicationUser> userManager
            )
            : base(context, dbLock)
        {
            _usermanagementService = usermanagementService;
            _userManager = userManager;
        }

        // ========== متدهای اختصاصی (همگی با قفل) ==========
        
        
        /// <summary>
        /// دریافت یک رسید بر اساس شماره فاکتور (حساس به حروف بزرگ و کوچک نیست)
        /// </summary>
        /// <param name="invoiceNumber">شماره فاکتور</param>
        /// <returns>شیء GoodsReceipt در صورت وجود، در غیر این صورت null</returns>
        public async Task<GoodsReceipt> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return null;
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet.FirstOrDefaultAsync(r => r.InvoiceNumber == invoiceNumber);
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// دریافت تمام رسیدهای مربوط به یک محصول مشخص
        /// </summary>
        /// <param name="productId">شناسه محصول (Guid)</param>
        /// <returns>لیستی از رسیدها به همراه اطلاعات محصول و تأمین‌کننده</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByProductIdAsync(Guid productId)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet
                    .Where(r => r.ProductId == productId)
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .ToListAsync();
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// دریافت تمام رسیدهای مربوط به یک تأمین‌کننده خاص
        /// </summary>
        /// <param name="supplierId">شناسه تأمین‌کننده</param>
        /// <returns>لیست رسیدها به همراه اطلاعات تأمین‌کننده</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(int supplierId)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet
                    .Where(r => r.SupplierId == supplierId)
                    .Include(r => r.Supplier)
                    .ToListAsync();
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// دریافت تمام رسیدهای مربوط به یک انبار خاص
        /// </summary>
        /// <param name="warehouseId">شناسه انبار</param>
        /// <returns>لیست رسیدها به همراه اطلاعات انبار</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet
                    .Where(r => r.WarehouseId == warehouseId)
                    .Include(r => r.Warehouse)
                    .ToListAsync();
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// دریافت رسیدهای ثبت‌شده در یک بازه زمانی مشخص
        /// </summary>
        /// <param name="startDate">تاریخ شروع (شمسی – میلادی در دیتابیس)</param>
        /// <param name="endDate">تاریخ پایان</param>
        /// <returns>لیست رسیدها مرتب شده بر اساس تاریخ دریافت (قدیم‌ترین اول)</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet
                    .Where(r => r.ReceiptDate >= startDate && r.ReceiptDate <= endDate)
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .OrderBy(r => r.ReceiptDate)
                    .ToListAsync();
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }
        /// <summary>
        /// دریافت تعداد مشخصی از آخرین رسیدهای **تأیید شده** (وضعیت 1) به همراه جزئیات (DTO)
        /// </summary>
        public async Task<List<GoodsReceiptDto>> GetTopApprovedReceiptsAsync(int topCount)
        {
            if (topCount <= 0) topCount = 20;

            try
            {
                var receipts = await _dbSet
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .Include(r => r.Warehouse)
                    .Where(r => r.Status == 1)
                    .OrderByDescending(r => r.ReceiptDate)
                    .Take(topCount)
                    .AsNoTracking()
                    .ToListAsync();

                if (receipts == null || !receipts.Any())
                    return new List<GoodsReceiptDto>();

                var userIds = receipts.Where(r => !string.IsNullOrEmpty(r.UserId)).Select(r => r.UserId).Distinct().ToList();
                Dictionary<string, string> userNames = new Dictionary<string, string>();

                if (userIds.Any())
                {
                    var users = await _userManager.Users
                        .Where(u => userIds.Contains(u.Id))
                        .Select(u => new { u.Id, u.FullName })
                        .ToListAsync();
                    userNames = users.ToDictionary(u => u.Id, u => u.FullName);
                }

                var result = new List<GoodsReceiptDto>();
                foreach (var receipt in receipts)
                {
                    string userName = (!string.IsNullOrEmpty(receipt.UserId) && userNames.ContainsKey(receipt.UserId))
                        ? userNames[receipt.UserId]
                        : "نامشخص";

                    result.Add(new GoodsReceiptDto
                    {
                        ReceiptId = receipt.ReceiptId,
                        ProductName = receipt.Product?.Name ?? "نامشخص",
                        SupplierName = receipt.Supplier?.Name ?? "نامشخص",
                        WarehouseName = receipt.Warehouse?.Name ?? "نامشخص",
                        UserFullName = userName,
                        Quantity = receipt.Quantity,
                        Unit = receipt.Unit,
                        UnitPrice = receipt.UnitPrice,
                        TotalPrice = receipt.Quantity * receipt.UnitPrice,
                        InvoiceNumber = receipt.InvoiceNumber,
                        InvoiceDate = receipt.InvoiceDate,
                        ReceiptDate = receipt.ReceiptDate,
                        StatusText = "تأیید شده",
                        BatchNumber = receipt.BatchNumber,
                        Description = receipt.Description,
                        ApprovedByFullName = receipt.ApprovedByFullName ?? "نامشخص"   // ✅ اضافه شد
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// دریافت یک رسید به همراه تمام جزئیات مرتبط (محصول، تأمین‌کننده، انبار)
        /// </summary>
        /// <param name="receiptId">شناسه رسید</param>
        /// <returns>شیء GoodsReceipt با اطلاعات وابسته</returns>
        public async Task<GoodsReceipt> GetReceiptWithDetailsAsync(int receiptId)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                return await _dbSet
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .Include(r => r.Warehouse)
                    .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// محاسبه قیمت خالص کل (با احتساب مالیات) برای یک محصول خاص بر اساس رسیدهای تأیید شده
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <returns>جمع قیمت خالص (decimal)</returns>
        public async Task<decimal> GetTotalNetPriceByProductAsync(Guid productId)
        {
            await DbLock.Semaphore.WaitAsync();
            try
            {
                var receipts = await _dbSet
                    .Where(r => r.ProductId == productId && r.Status == 1)
                    .ToListAsync();
                return receipts.Sum(r => r.NetPrice);
            }
            finally
            {
                DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// تأیید رسید ورودی و افزایش موجودی محصول و انبار.
        ///
        /// نکته مهم درباره همزمانی: موجودی با «خواندن، جمع کردن، نوشتن» به‌روزرسانی نمی‌شود.
        /// هر سه تغییر با ExecuteUpdateAsync انجام می‌شود که به یک دستور UPDATE اتمیک
        /// در دیتابیس تبدیل می‌شود. بنابراین اگر دو کاربر همزمان دو رسید را تأیید کنند،
        /// هر دو افزایش دقیقاً یک بار اعمال می‌شود و هیچ تغییری گم نمی‌شود.
        /// این تضمین بین چند سرور و بین نسخه وب و دسکتاپ هم برقرار است.
        /// </summary>
        public async Task<(bool Success, string Message)> ApproveReceiptAsync(int receiptId, string currentUserId)
        {
            // 1. اعتبارسنجی ورودی
            if (string.IsNullOrEmpty(currentUserId))
                return (false, "کاربر جاری شناسایی نشد. لطفاً وارد شوید.");

            // 2. خواندن اطلاعات لازم (بدون قفل و فقط برای اعتبارسنجی اولیه)
            var receipt = await _dbSet.FirstOrDefaultAsync(r => r.ReceiptId == receiptId);

            if (receipt == null)
                return (false, "رسید یافت نشد.");

            if (receipt.Status == 1)
                return (false, "این رسید قبلاً تأیید شده است.");

            if (!receipt.WarehouseId.HasValue)
                return (false, "انبار این رسید انتخاب نشده است.");

            // 3. دریافت کاربر تأییدکننده
            var user = await _userManager.FindByIdAsync(currentUserId)
                       ?? await _usermanagementService.FindUserByIdAsync(currentUserId);

            if (user == null)
                return (false, "کاربر تأییدکننده در دیتابیس یافت نشد.");

            var quantity = receipt.Quantity;
            var productId = receipt.ProductId;
            var warehouseId = receipt.WarehouseId.Value;
            var approverName = user.FullName ?? user.UserName ?? "نامشخص";

            // در صورت خطای موقت دیتابیس، کل عملیات به‌صورت خودکار تکرار می‌شود.
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 4. تغییر وضعیت با شرط Status = 0.
                    // این شرط تضمین می‌کند دو کاربر همزمان فقط یکی بتواند رسید را تأیید کند.
                    var statusRows = await _dbSet
                        .Where(r => r.ReceiptId == receiptId && r.Status == 0)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(r => r.Status, (byte)1)
                            .SetProperty(r => r.ApprovedByUserId, currentUserId)
                            .SetProperty(r => r.ApprovedByFullName, approverName));

                    if (statusRows == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "این رسید قبلاً تأیید شده است.");
                    }

                    // 5. افزایش اتمیک موجودی محصول
                    await _context.Set<Product>()
                        .Where(p => p.ProductId == productId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.Number, p => (p.Number ?? 0) + quantity));

                    // 6. افزایش اتمیک موجودی انبار
                    await _context.Set<Warehouse>()
                        .Where(w => w.WarehouseId == warehouseId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(w => w.Number, w => w.Number + quantity));

                    await transaction.CommitAsync();

                    return (true, "رسید با موفقیت تأیید شد و موجودی‌ها به‌روزرسانی شدند.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"خطا در تأیید رسید: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// لغو یک رسید (تغییر وضعیت به 2)
        /// </summary>
        /// <param name="receiptId">شناسه رسید</param>
        /// <returns>true در صورت موفقیت، false اگر رسید یافت نشد</returns>
        public async Task<bool> CancelReceiptAsync(int receiptId)
        {
            var receipt = await GetByIdAsync(receiptId);
            if (receipt == null) return false;
            receipt.Status = 2;
            await UpdateAsync(receipt);
            return true;
        }

        /// <summary>
        /// به‌روزرسانی یک رسید (با قفل)
        /// </summary>
        /// <param name="entity">شیء GoodsReceipt با مقادیر جدید</param>
        /// <returns>شیء به‌روز شده</returns>
        public override async Task<GoodsReceipt> UpdateAsync(GoodsReceipt entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            //await DbLock.Semaphore.WaitAsync();
            try
            {
                return await base.UpdateAsync(entity);
            }
            finally
            {
                //DbLock.Semaphore.Release();
            }
        }

        // ========== متدهای DTO (نیاز به قفل دارند چون به دیتابیس دسترسی دارند) ==========

        /// <summary>
        /// دریافت تعداد مشخصی از آخرین رسیدها به همراه جزئیات (نام محصول، تأمین‌کننده، انبار، کاربر و ...)
        /// </summary>
        /// <param name="topCount">تعداد رکورد مورد نظر (پیش‌فرض ۲۰)</param>
        /// <returns>لیستی از GoodsReceiptDto</returns>
        public async Task<List<GoodsReceiptDto>> GetTopReceiptsWithDetailsAsync(int topCount)
        {
            if (topCount <= 0) topCount = 20;

            // (اختیاری) در صورت نیاز قفل را فعال کنید
            // await DbLock.Semaphore.WaitAsync();
            try
            {
                var receipts = await _dbSet
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .Include(r => r.Warehouse)
                    .Where(r => r.Status == 0)
                    .OrderByDescending(r => r.ReceiptDate)
                    .Take(topCount)
                    .AsNoTracking()
                    .ToListAsync();

                if (receipts == null || !receipts.Any())
                    return new List<GoodsReceiptDto>();

                var userIds = receipts.Where(r => !string.IsNullOrEmpty(r.UserId)).Select(r => r.UserId).Distinct().ToList();
                Dictionary<string, string> userNames = new Dictionary<string, string>();

                if (userIds.Any())
                {
                   
                    var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { u.Id, u.FullName }).ToListAsync();
                    userNames = users.ToDictionary(u => u.Id, u => u.FullName);
                }

                var result = new List<GoodsReceiptDto>();
                foreach (var receipt in receipts)
                {
                    string userName = (!string.IsNullOrEmpty(receipt.UserId) && userNames.ContainsKey(receipt.UserId))
                        ? userNames[receipt.UserId]
                        : "نامشخص";

                    result.Add(new GoodsReceiptDto
                    {
                        ReceiptId = receipt.ReceiptId,
                        ProductName = receipt.Product?.Name ?? "نامشخص",
                        SupplierName = receipt.Supplier?.Name ?? "نامشخص",
                        WarehouseName = receipt.Warehouse?.Name ?? "نامشخص",
                        UserFullName = userName,
                        Quantity = receipt.Quantity,
                        Unit = receipt.Unit,
                        UnitPrice = receipt.UnitPrice,
                        TotalPrice = receipt.Quantity * receipt.UnitPrice,
                        InvoiceNumber = receipt.InvoiceNumber,
                        InvoiceDate = receipt.InvoiceDate,
                        ReceiptDate = receipt.ReceiptDate,
                        StatusText = receipt.Status == 0 ? "در انتظار تأیید" : (receipt.Status == 1 ? "تأیید شده" : "لغو شده"),
                        BatchNumber = receipt.BatchNumber,
                        Description = receipt.Description,
                        ApprovedByFullName= receipt.ApprovedByFullName,
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                // می‌توانید خطا را لاگ کنید
                throw;
            }
            finally
            {
                // DbLock.Semaphore.Release();
            }
        }

        /// <summary>
        /// جستجوی رسیدها بر اساس کلمه کلیدی (در شماره فاکتور، نام محصول، نام تأمین‌کننده)
        /// </summary>
        /// <param name="keyword">عبارت جستجو</param>
        /// <param name="topCount">حداکثر تعداد نتیجه (پیش‌فرض ۵۰)</param>
        /// <returns>لیستی از GoodsReceiptDto</returns>
        public async Task<List<GoodsReceiptDto>> SearchReceiptsAsync(string keyword, int topCount = 50)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetTopReceiptsWithDetailsAsync(topCount);

            keyword = keyword.Trim().ToLower();

            try
            {
                var query = _dbSet
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .Include(r => r.Warehouse)
                    .Where(r => r.Status == 0 && // ✅ فقط رسیدهای در انتظار تأیید
                                (r.InvoiceNumber.ToLower().Contains(keyword) ||
                                 (r.Product != null && r.Product.Name.ToLower().Contains(keyword)) ||
                                 (r.Supplier != null && r.Supplier.Name.ToLower().Contains(keyword))))
                    .OrderByDescending(r => r.ReceiptDate)
                    .Take(topCount)
                    .AsNoTracking();

                var receipts = await query.ToListAsync();
                var result = new List<GoodsReceiptDto>();

                foreach (var receipt in receipts)
                {
                    // دریافت نام کاربر با مدیریت خطا
                    string userName = "نامشخص";
                    if (!string.IsNullOrEmpty(receipt.UserId))
                    {
                        try
                        {
                            userName = await _usermanagementService.GetUserNameByIdAsync(receipt.UserId);
                            if (string.IsNullOrEmpty(userName))
                                userName = "نامشخص";
                        }
                        catch
                        {
                            userName = "نامشخص";
                        }
                    }

                    result.Add(new GoodsReceiptDto
                    {
                        ReceiptId = receipt.ReceiptId,
                        ProductName = receipt.Product?.Name ?? "نامشخص",
                        SupplierName = receipt.Supplier?.Name ?? "نامشخص",
                        WarehouseName = receipt.Warehouse?.Name ?? "نامشخص",
                        UserFullName = userName,
                        Quantity = receipt.Quantity,
                        Unit = receipt.Unit,
                        UnitPrice = receipt.UnitPrice,
                        TotalPrice = receipt.Quantity * receipt.UnitPrice,
                        InvoiceNumber = receipt.InvoiceNumber,
                        InvoiceDate = receipt.InvoiceDate,
                        ReceiptDate = receipt.ReceiptDate,
                        StatusText = receipt.Status == 0 ? "در انتظار تأیید" : (receipt.Status == 1 ? "تأیید شده" : "لغو شده"),
                        BatchNumber = receipt.BatchNumber,
                        Description = receipt.Description
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                // لاگ خطا در صورت نیاز
                return new List<GoodsReceiptDto>();
            }
        }

        public async Task<List<GoodsReceiptDto>> SearchReceipts(string keyword, int topCount = 50)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetTopReceiptsWithDetailsAsync(topCount);

            keyword = keyword.Trim().ToLower();

            try
            {
                var query = _dbSet
                    .Include(r => r.Product)
                    .Include(r => r.Supplier)
                    .Include(r => r.Warehouse)
                    .Where(r => r.Status == 1 && // ✅ فقط رسید های تایید شده
                                (r.InvoiceNumber.ToLower().Contains(keyword) ||
                                 (r.Product != null && r.Product.Name.ToLower().Contains(keyword)) ||
                                 (r.Supplier != null && r.Supplier.Name.ToLower().Contains(keyword))))
                    .OrderByDescending(r => r.ReceiptDate)
                    .Take(topCount)
                    .AsNoTracking();

                var receipts = await query.ToListAsync();
                var result = new List<GoodsReceiptDto>();

                foreach (var receipt in receipts)
                {
                    // دریافت نام کاربر با مدیریت خطا
                    string userName = "نامشخص";
                    if (!string.IsNullOrEmpty(receipt.UserId))
                    {
                        try
                        {
                            userName = await _usermanagementService.GetUserNameByIdAsync(receipt.UserId);
                            if (string.IsNullOrEmpty(userName))
                                userName = "نامشخص";
                        }
                        catch
                        {
                            userName = "نامشخص";
                        }
                    }

                    result.Add(new GoodsReceiptDto
                    {
                        ReceiptId = receipt.ReceiptId,
                        ProductName = receipt.Product?.Name ?? "نامشخص",
                        SupplierName = receipt.Supplier?.Name ?? "نامشخص",
                        WarehouseName = receipt.Warehouse?.Name ?? "نامشخص",
                        UserFullName = userName,
                        Quantity = receipt.Quantity,
                        Unit = receipt.Unit,
                        UnitPrice = receipt.UnitPrice,
                        TotalPrice = receipt.Quantity * receipt.UnitPrice,
                        InvoiceNumber = receipt.InvoiceNumber,
                        InvoiceDate = receipt.InvoiceDate,
                        ReceiptDate = receipt.ReceiptDate,
                        StatusText = receipt.Status == 0 ? "در انتظار تأیید" : (receipt.Status == 1 ? "تأیید شده" : "لغو شده"),
                        BatchNumber = receipt.BatchNumber,
                        Description = receipt.Description
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                // لاگ خطا در صورت نیاز
                return new List<GoodsReceiptDto>();
            }
        }

    }
}
