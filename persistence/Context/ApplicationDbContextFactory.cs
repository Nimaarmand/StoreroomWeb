using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistans.Context
{
    /// <summary>
    /// این کلاس فقط برای ابزارهای خط فرمان (dotnet ef) استفاده می‌شود و در زمان اجرا
    /// وارد جریان برنامه نمی‌گردد. وجودش باعث می‌شود تولید migration به پروژه
    /// WinForms وابسته نباشد و پس از مهاجرت به وب هم ابزارها کار کنند.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                "Server=.;Database=StoreroomDB;Trusted_Connection=True;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
