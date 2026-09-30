using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Web.Infrastructure;

/// <summary>
/// ساخت دیتابیس و داده‌های اولیه در اولین اجرا.
/// شامل جدول‌های Identity، نقش‌ها و کاربر مدیر.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var identityContext = provider.GetRequiredService<IdentityContext>();
        var appContext = provider.GetRequiredService<ApplicationDbContext>();

        await identityContext.Database.MigrateAsync();
        await appContext.Database.MigrateAsync();

        await SeedRolesAsync(provider.GetRequiredService<RoleManager<Role>>());
        await SeedAdminUserAsync(
            provider.GetRequiredService<UserManager<ApplicationUser>>(),
            provider.GetRequiredService<IConfiguration>(),
            provider.GetRequiredService<IWebHostEnvironment>());

        // کاربران و داده‌های نمونه فقط در توسعه ساخته می‌شوند. ساختن آن‌ها در محیط
        // عملیاتی یعنی داشتن حساب‌هایی با رمز ثابت و قابل حدس.
        if (provider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            await SeedDemoUsersAsync(provider.GetRequiredService<UserManager<ApplicationUser>>());
            await DemoDataSeeder.SeedAsync(services);
        }
    }

    private static async Task SeedDemoUsersAsync(UserManager<ApplicationUser> userManager)
    {
        (string UserName, string Password, string FullName, string Role)[] demoUsers =
        {
            ("manager", "Manager@123", "سرپرست انبار", "Manager"),
            ("user", "User@123", "کاربر انبار", "User"),
        };

        foreach (var (userName, password, fullName, role) in demoUsers)
        {
            var existing = await userManager.FindByNameAsync(userName);
            if (existing == null)
            {
                var created = await userManager.CreateAsync(new ApplicationUser
                {
                    UserName = userName,
                    Email = $"{userName}@storeroom.local",
                    FullName = fullName,
                    EmailConfirmed = true,
                }, password);

                if (!created.Succeeded)
                    throw new InvalidOperationException(
                        $"ساخت کاربر نمونه «{userName}» ناموفق بود: " +
                        string.Join("، ", created.Errors.Select(e => e.Description)));

                existing = await userManager.FindByNameAsync(userName);
            }

            if (existing != null && !await userManager.IsInRoleAsync(existing, role))
                await userManager.AddToRolesAsync(existing, [role]);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        (string Name, string Description)[] roles =
        {
            ("Admin", "دسترسی کامل به همه بخش‌ها و تنظیمات سیستم"),
            ("Manager", "تأیید نهایی رسید و حواله، گزارش‌گیری و ویرایش اطلاعات پایه"),
            ("User", "ثبت رسید و حواله و مشاهده موجودی؛ بدون تأیید نهایی"),
        };

        foreach (var (name, description) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
                await roleManager.CreateAsync(new Role { Name = name, Discription = description });
        }
    }

    /// <summary>
    /// ساخت کاربر مدیر در اولین اجرا.
    /// نام کاربری و رمز از پیکربندی خوانده می‌شود تا رمز ثابت داخل کد نماند.
    /// در محیط عملیاتی، نبود این تنظیمات یک خطاست نه یک هشدار: ساخت خودکار
    /// مدیر با رمز پیش‌فرضِ شناخته‌شده، عملاً دسترسی کامل به سیستم را عمومی می‌کند.
    /// </summary>
    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        const string adminUserName = "admin";

        if (await userManager.FindByNameAsync(adminUserName) != null)
            return;

        var password = configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(password))
        {
            if (!environment.IsDevelopment())
                throw new InvalidOperationException(
                    "تنظیم Seed:AdminPassword تعریف نشده است. آن را با دستور زیر ثبت کنید: " +
                    "dotnet user-secrets set \"Seed:AdminPassword\" \"<رمز-قوی>\" " +
                    "(یا به‌عنوان متغیر محیطی Seed__AdminPassword).");

            // فقط در توسعه، تا اولین اجرا بدون تنظیم اضافه ممکن باشد.
            password = "Admin@123";
        }

        var admin = new ApplicationUser
        {
            UserName = adminUserName,
            Email = configuration["Seed:AdminEmail"] ?? "admin@storeroom.local",
            FullName = "مدیر سیستم",
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                "ساخت کاربر مدیر ناموفق بود: " +
                string.Join("، ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRolesAsync(admin, ["Admin"]);
    }
}
