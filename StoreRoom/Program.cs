using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Application.Features.Implementation.Category_Service;
using Application.Features.Implementation.Customer_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Application.Features.Implementation.GoodsIssue_Service;
using Application.Features.Implementation.GoodsReceipt_Service;
using Application.Features.Implementation.Product_Service;
using Application.Features.Implementation.Supplier_Service;
using Application.Features.Implementation.Usermanagement_Service;
using Application.Features.Implementation.Warehouse_Service;
using Domain.Entity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistans.Context;
using StoreRoom.Data;
using StoreRoom.Forms;
using StoreRoom.Utilities;
using System;
using System.IO;
using System.Windows.Forms;

namespace StoreRoom
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static string CurrentUserId { get; set; }

        [STAThread]
        static void Main()
        {
            // ============================================================
            // ✅ دو خط زیر برای رفع مشکل DPI اضافه شده‌اند
            // ============================================================
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Logger.Log("========================================");
                Logger.Log("🚀 برنامه StoreRoom شروع شد");
                Logger.LogSystemInfo();
                Logger.Log($"📂 مسیر اجرا: {AppDomain.CurrentDomain.BaseDirectory}");
                Logger.Log($"🧠 نسخه دات‌نت: {Environment.Version}");

                Logger.Log("🔍 در حال بررسی لایسنس...");
                var license = LicenseManager.ValidateLicense();
                if (license == null)
                {
                    string machineCode = HardwareHelper.GetMachineCode();
                    Logger.Log($"❌ لایسنس نامعتبر - کد ماشین: {machineCode}");
                    MessageBox.Show(
                        $"لایسنس معتبر نیست!\n\n" +
                        $"کد ماشین شما:\n{machineCode}\n\n" +
                        "لطفاً این کد را برای دریافت لایسنس به پشتیبانی ارسال کنید.",
                        "خطای لایسنس",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Logger.Log($"✅ لایسنس معتبر - وضعیت: {license.GetStatusText()}");

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=StoreroomDB;Trusted_Connection=True;TrustServerCertificate=True;";
                ApplicationConfiguration.Initialize();

                // ========== ساخت دیتابیس و جدول‌ها در اولین اجرا ==========
                Logger.Log("🔄 بررسی و ساخت دیتابیس روی LocalDB...");
                try
                {
                    var identityOptionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
                    identityOptionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
                    using (var identityContext = new IdentityContext(identityOptionsBuilder.Options))
                    {
                        identityContext.Database.Migrate();
                        Logger.Log("✅ دیتابیس Identity با موفقیت ساخته/بروزرسانی شد");
                    }

                    var appOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    appOptionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
                    using (var appContext = new ApplicationDbContext(appOptionsBuilder.Options))
                    {
                        appContext.Database.Migrate();
                        Logger.Log("✅ دیتابیس اپلیکیشن با موفقیت ساخته/بروزرسانی شد");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ساخت دیتابیس");
                    MessageBox.Show(
                        $"خطا در ساخت دیتابیس:\n{ex.Message}\n\n" +
                        $"فایل لاگ در مسیر زیر ذخیره شده است:\n{Logger.GetTodayLogPath()}",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // ========== اجرای Seeder ==========
                Logger.Log("🔄 شروع سید دیتابیس...");
                try
                {
                    var seeder = new IdentityDatabaseSeeder(connectionString);
                    seeder.SeedAllRolesAsync().GetAwaiter().GetResult();
                    seeder.SeedAdminUserAsync().GetAwaiter().GetResult();
                    Logger.Log("✅ سید دیتابیس با موفقیت انجام شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در سید دیتابیس");
                    MessageBox.Show(
                        $"خطا در سید دیتابیس:\n{ex.Message}\n\n" +
                        $"فایل لاگ در مسیر زیر ذخیره شده است:\n{Logger.GetTodayLogPath()}",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var services = new ServiceCollection();

                Logger.Log("🔄 ثبت DbContextها...");
                try
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
                    services.AddDbContext<IdentityContext>(options =>
                        options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
                    services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
                    Logger.Log("✅ DbContextها با موفقیت ثبت شدند");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ثبت DbContextها");
                    throw;
                }

                Logger.Log("🔄 ثبت Identity...");
                try
                {
                    services.AddIdentityCore<ApplicationUser>()
                        .AddRoles<Role>()
                        .AddEntityFrameworkStores<IdentityContext>()
                        .AddDefaultTokenProviders();
                    Logger.Log("✅ Identity با موفقیت ثبت شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ثبت Identity");
                    throw;
                }

                Logger.Log("🔄 ثبت Data Protection...");
                try
                {
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "StoreroomKeys")))
                        .SetApplicationName("StoreroomApp");
                    Logger.Log("✅ Data Protection با موفقیت ثبت شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ثبت Data Protection");
                    throw;
                }

                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.User.RequireUniqueEmail = false;
                });

                Logger.Log("🔄 ثبت سرویس‌های سفارشی...");
                try
                {
                    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                    services.AddScoped<WarehouseService>();
                    services.AddScoped<SupplierService>();
                    services.AddScoped<GoodsReceiptService>();
                    services.AddScoped<GoodsIssueService>();
                    services.AddScoped<CustomerService>();
                    services.AddScoped<ProductService>();
                    services.AddScoped<CategoryService>();
                    services.AddScoped<UsermanagementService>();
                    Logger.Log("✅ سرویس‌های سفارشی با موفقیت ثبت شدند");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ثبت سرویس‌های سفارشی");
                    throw;
                }

                Logger.Log("🔄 ثبت فرم‌ها...");
                try
                {
                    services.AddTransient<Form1>();
                    services.AddTransient<Form2>();
                    services.AddTransient<Form3>();
                    services.AddTransient<Form4>();
                    services.AddTransient<Form5>();
                    services.AddTransient<Form6>();
                    services.AddTransient<Form7>();
                    services.AddTransient<Form8>();
                    services.AddTransient<Form9>();
                    services.AddTransient<Form10>();
                    services.AddTransient<Form11>();
                    services.AddTransient<Form12>();
                    services.AddTransient<Form13>();
                    services.AddTransient<Form14>();
                    services.AddTransient<Form15>();
                    services.AddTransient<Form16>();
                    services.AddTransient<Form17>();
                    services.AddTransient<Form18>();
                    services.AddTransient<Form19>();
                    services.AddTransient<Form20>();
                    services.AddTransient<Form21>();
                    services.AddTransient<Form22>();
                    services.AddTransient<Form23>();
                    services.AddTransient<Form24>();
                    services.AddTransient<Form27>();
                    services.AddTransient<Settings>();
                    services.AddTransient<FormMachineCode>();
                    services.AddTransient<ContactUs>();
                    Logger.Log("✅ فرم‌ها با موفقیت ثبت شدند");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ثبت فرم‌ها");
                    throw;
                }

                Logger.Log("🔄 ساخت ServiceProvider...");
                try
                {
                    ServiceProvider = services.BuildServiceProvider();
                    Logger.Log("✅ ServiceProvider با موفقیت ساخته شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در ساخت ServiceProvider");
                    throw;
                }

                Logger.Log("🔄 راه‌اندازی پشتیبان‌گیری خودکار...");
                try
                {
                    string backupFolder = @"D:\StoreroomBackups";
                    if (!Directory.Exists(@"D:\"))
                    {
                        backupFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Storeroom", "Backups");
                    }
                    AutoBackup.Start(connectionString, backupFolder);
                    Logger.Log($"✅ پشتیبان‌گیری خودکار راه‌اندازی شد - مسیر: {backupFolder}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در راه‌اندازی پشتیبان‌گیری خودکار");
                }

                // ========== نمایش فرم 27 ==========
                Logger.Log("🔄 نمایش فرم 27...");
                try
                {
                    var form27 = ServiceProvider.GetRequiredService<Form27>();
                    form27.ShowDialog();
                    Logger.Log("✅ فرم 27 نمایش داده شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در نمایش فرم 27");
                    MessageBox.Show(
                        $"خطا در نمایش فرم 27:\n{ex.Message}",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // ========== اجرای فرم لاگین (Form14) ==========
                Logger.Log("🔄 اجرای فرم لاگین...");
                try
                {
                    var loginForm = ServiceProvider.GetRequiredService<Form14>();
                    System.Windows.Forms.Application.Run(loginForm);
                    Logger.Log("✅ فرم لاگین اجرا شد");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "خطا در اجرای فرم لاگین");
                    MessageBox.Show(
                        $"خطا در اجرای فرم لاگین:\n{ex.Message}",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                Logger.Log("🛑 برنامه به پایان رسید");
                Logger.Log("========================================");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "خطای کلی در اجرای برنامه");
                MessageBox.Show(
                    $"خطای غیرمنتظره‌ای در برنامه رخ داد:\n\n" +
                    $"{ex.Message}\n\n" +
                    $"فایل لاگ در مسیر زیر ذخیره شده است:\n" +
                    $"{Logger.GetTodayLogPath()}\n\n" +
                    "لطفاً این فایل را برای پشتیبانی ارسال کنید.",
                    "خطای برنامه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
//System.Windows.Forms.Application.Run(loginForm);