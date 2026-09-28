using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Features.Definition.Context
{
    /// <summary>
    /// لایه انتزاعی دسترسی به دیتابیس برای لایه Application.
    /// پیاده‌سازی‌ها در پروژه persistence قرار دارند.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// دریافت مجموعه موجودیت برای سرویس‌های عمومی (GenericRepository).
        /// </summary>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// دسترسی به تراکنش و استراتژی اجرای EF Core.
        /// سرویس‌های تأیید رسید/حواله برای کار با تراکنش از این پراپرتی استفاده می‌کنند
        /// و دیگر نیازی به cast کردن به DbContext ندارند.
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// ردیابی تغییرات. برای آزادکردن نمونه‌ای که پیش‌تر با همان کلید
        /// بارگذاری شده لازم است؛ در غیر این صورت Update با خطای
        /// «دو نمونه با همان کلید» شکست می‌خورد.
        /// </summary>
        ChangeTracker ChangeTracker { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
