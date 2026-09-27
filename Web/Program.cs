using Application.Features.Definition.Category_Service;
using Application.Features.Definition.Context;
using Application.Features.Definition.Customer_Service;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.GoodsIssue_Service;
using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Definition.Product_Service;
using Application.Features.Definition.Supplier_Service;
using Application.Features.Definition.Usermanagement_Service;
using Application.Features.Definition.Warehouse_Service;
using Application.Features.Implementation.Category_Service;
using Application.Features.Implementation.Common;
using Application.Features.Implementation.Customer_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Application.Features.Implementation.GoodsIssue_Service;
using Application.Features.Implementation.GoodsReceipt_Service;
using Application.Features.Implementation.Product_Service;
using Application.Features.Implementation.Supplier_Service;
using Application.Features.Implementation.Usermanagement_Service;
using Application.Features.Implementation.Warehouse_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistans.Context;
using Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// دیتابیس
// تا وقتی برنامه در حال توسعه است روی دیتابیس جدا کار می‌کند تا داده‌های
// نسخه دسکتاپ دست‌نخورده بماند. برای استقرار نهایی فقط کافی است همین یک
// رشته در appsettings.json عوض شود.
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("رشته اتصال DefaultConnection تعریف نشده است.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

// ============================================================
// احراز هویت: Identity + Cookie
// ============================================================
builder.Services
    .AddIdentity<ApplicationUser, Role>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "Storeroom.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// ============================================================
// مجوزدهی بر پایه نقش
// نقش‌ها همان سه نقش موجود در IdentityDatabaseSeeder هستند:
// Admin، Manager، User
// ============================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.AdminOnly, policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy(AuthPolicies.ManagerOrAdmin, policy =>
        policy.RequireRole("Admin", "Manager"));

    // ثبت رسید و حواله نیاز به حداقل نقش کاربر عادی دارد
    options.AddPolicy(AuthPolicies.CanManageInventory, policy =>
        policy.RequireRole("Admin", "Manager", "User"));

    // ویرایش و حذف مراجع پایه (کالا، مشتری، تامین‌کننده، انبار، دسته‌بندی)
    options.AddPolicy(AuthPolicies.CanEditMasterData, policy =>
        policy.RequireRole("Admin", "Manager"));

    // تأیید نهایی رسید/حواله = عملیات حساس مالی
    options.AddPolicy(AuthPolicies.CanApprove, policy =>
        policy.RequireRole("Admin", "Manager"));
});

builder.Services.AddLocalization();

// ============================================================
// MVC + فارسی و تاریخ شمسی
// ============================================================
builder.Services.AddControllersWithViews();

const string persianCulture = "fa-IR";
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supported = new[] { new System.Globalization.CultureInfo(persianCulture) };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(persianCulture);
    options.SupportedCultures = supported;
    options.SupportedUICultures = supported;
});

// ============================================================
// لایه Application
// فقط روی اینترفیس‌ها ثبت می‌شود. کنترلرها هیچ وابستگی مستقیمی به
// کلاس‌های concrete ندارند تا تست و جایگزینی ساده باشد.
// ============================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDbLock, DbLock>();

builder.Services.AddScoped<IUserManagement, UsermanagementService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
builder.Services.AddScoped<IGoodsIssueService, GoodsIssueService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// ارقام فارسی، اعتبارسنجی و سایر کمکی‌های لایه وب
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<IReceiptValidator, ReceiptValidator>();
builder.Services.AddScoped<IIssueValidator, IssueValidator>();

var app = builder.Build();

// ============================================================
// ساخت خودکار دیتابیس و داده‌های اولیه
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await DbInitializer.InitializeAsync(scope.ServiceProvider);
        logger.LogInformation("دیتابیس و داده‌های اولیه آماده است.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "خطا در آماده‌سازی دیتابیس.");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
