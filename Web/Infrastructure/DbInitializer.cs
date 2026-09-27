using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistans.Context;

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
        await SeedAdminUserAsync(provider.GetRequiredService<UserManager<ApplicationUser>>());
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

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminUserName = "admin";
        const string adminPassword = "Admin@123";

        if (await userManager.FindByNameAsync(adminUserName) != null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminUserName,
            Email = "admin@storeroom.local",
            FullName = "مدیر سیستم",
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                "ساخت کاربر مدیر ناموفق بود: " +
                string.Join("، ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRolesAsync(admin, ["Admin"]);
    }
}
