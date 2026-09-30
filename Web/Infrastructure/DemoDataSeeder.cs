using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Web.Infrastructure;

/// <summary>
/// داده نمایشی برای ارائه به خریدار و توسعه.
/// <para>
/// فقط در حالت Development اجرا می‌شود. در محیط عملیاتی یعنی دیتابیس خالی
/// نماند، ولی انبار پر از کالای ساختگی و فاکتور الکی.
/// </para>
/// <para>
/// موجودی کالا از تفاضل رسیدهای تأییدشده و حواله‌های تأییدشده به دست می‌آید
/// (همان کاری که سرویس‌ها هنگام تأیید انجام می‌دهند)، پس هر سه با هم سازگارند.
/// </para>
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();

        // اگر داده‌ای هست، دست نمی‌زنیم. Seeder باید تکرارپذیر و بی‌خطر باشد.
        if (await context.Categories.AnyAsync())
            return;

        // ---------- انبارها ----------
        var central = new Warehouse
        {
            Name = "انبار مرکزی",
            Location = "تهران، خیابان آزادی، پلاک ۱۲۰",
            Min = 50,
            Max = 50_000,
            Status = "فعال"
        };
        var branch = new Warehouse
        {
            Name = "انبار شعبه غرب",
            Location = "تهران، خیابان اشرفی، پلاک ۴۵",
            Min = 20,
            Max = 20_000,
            Status = "فعال"
        };
        context.Warehouses.AddRange(central, branch);
        await context.SaveChangesAsync();

        // ---------- تأمین‌کنندگان ----------
        var suppliers = new[]
        {
            new Supplier { Name = "شرکت پخش غذای پارسیان", Phone = "02166554433", Address = "تهران، مهرآباد", EconomicCode = "1012345678" },
            new Supplier { Name = "بازرگانی آریا تجارت",    Phone = "02166778899", Address = "تهران، شهرک آزادی", EconomicCode = "1023456789" },
            new Supplier { Name = "تولیدی پاک‌دست",          Phone = "03136554422", Address = "اصفهان، شهرک صنعتی", EconomicCode = "1034567890" },
            new Supplier { Name = "فروشگاه مرکزی کالای شیمیایی", Phone = "02133445566", Address = "تهران، خیابان جمهوری", EconomicCode = "1045678901" },
        };
        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();

        // ---------- مشتریان ----------
        var customers = new[]
        {
            new Customer { Name = "فروشگاه زنجیره‌ای بهار",     Phone = "09121234567", Address = "تهران، سعادت‌آباد", MarketName = "بهار" },
            new Customer { Name = "تعاونی مصرف کارکنان",        Phone = "09122345678", Address = "تهران، ونک", MarketName = "تعاونی کارکنان" },
            new Customer { Name = "رستوران سنتی گلستان",        Phone = "09123456789", Address = "تهران، بازار", MarketName = "گلستان" },
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        // ---------- دسته‌بندی‌ها (با یک زیر‌دسته، تا درخت دسته‌بندی دیده شود) ----------
        var beverages = new Category { Name = "خوراکی و نوشیدنی", IsActive = true, Description = "کالاهای غذایی و نوشیدنی" };
        var hygiene  = new Category { Name = "شوینده و بهداشتی", IsActive = true };
        var spare    = new Category { Name = "لوازم یدکی",        IsActive = true };
        var stationery = new Category { Name = "کاغذ و لوازم‌التحریر", IsActive = true };
        context.Categories.AddRange(beverages, hygiene, spare, stationery);
        await context.SaveChangesAsync();

        var drinks = new Category
        {
            Name = "نوشیدنی",
            ParentId = beverages.CategoryId,
            IsActive = true
        };
        context.Categories.Add(drinks);
        await context.SaveChangesAsync();

        // ---------- کالاها ----------
        // Number در این مرحله صفر است و در انتها از روی رسید/حواله محاسبه می‌شود.
        var products = new List<Product>
        {
            New("آب معدنی ۱.۵ لیتری",      drinks.CategoryId,     "بطری",  "6221001100011", 100, 200),
            New("نوشابه گازدار ۳۳۰ میلی",   drinks.CategoryId,     "قوطی",  "6221001100028", 200, 400),
            New("آب پرتقال ۱ لیتری",         drinks.CategoryId,     "بطری",  "6221001100035", 80,  160),
            New("برنج ایرانی درجه یک",       null,                  "کیلوگرم", "6221001100042", 500, 2000),
            New("روغن نباتی ۱ لیتری",       null,                  "بطری",  "6221001100059", 150, 300),
            New("شوینده مایع ظرفشویی",       hygiene.CategoryId,    "بطری",  "6221001100066", 120, 260),
            New("پودر لباسشویی ۳ کیلویی",    hygiene.CategoryId,    "بسته",  "6221001100073", 60,  140),
            New("دستمال کاغذی ۱۲ بسته",     hygiene.CategoryId,    "بسته",  "6221001100080", 90,  220),
            New("فیلتر روغنی خودرو",         spare.CategoryId,      "عدد",   "6221001100097", 40,  100),
            New("لنت ترمز جلو",              spare.CategoryId,      "عدد",   "6221001100103", 30,  80),
            New("دفترچه یادداشت ۱۰۰ برگ",  stationery.CategoryId, "عدد",   "6221001100110", 200, 500),
            New("خودکار آبی",               stationery.CategoryId, "عدد",   "6221001100127", 300, 800),
        };
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // ---------- رسیدها و حواله‌های تأییدشده ----------
        // تاریخ‌ها روی ۹۰ روز گذشته پخش شده‌اند تا نمودارها و بازه‌های گزارش
        // (این هفته / این ماه / سه ماه) داده داشته باشند.
        var today = DateTime.Today;
        var admin = provider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
        var adminUser = await admin.FindByNameAsync("admin");
        var userId = adminUser?.Id;
        var userName = adminUser?.FullName ?? "مدیر سیستم";

        var receipts = new List<GoodsReceipt>();
        var issues = new List<GoodsIssue>();
        var receiptNumber = 1000;

        // (شاخص کالا، مقدار، شاخص انبار، شاخص تأمین‌کننده، چند روز پیش)
        (int Product, int Qty, int Warehouse, int Supplier, int DaysAgo)[] receiptPlan =
        {
            (0,  500, 0, 1, 88), (1,  800, 0, 1, 86), (2,  300, 0, 1, 85),
            (3, 1200, 0, 0, 78), (4,  400, 0, 0, 76), (5,  350, 0, 3, 70),
            (6,  250, 0, 3, 68), (7,  450, 0, 3, 66), (8,  200, 0, 1, 55),
            (9,  150, 0, 1, 54), (10, 600, 0, 0, 45), (11, 900, 0, 0, 44),
            (3,  600, 0, 0, 30), (0,  300, 0, 1, 28), (1,  400, 0, 1, 25),
            (5,  200, 0, 3, 20), (7,  250, 0, 3, 18), (10, 400, 0, 0, 12),
            (2,  200, 0, 1, 9),  (11, 500, 0, 0, 7),  (6,  120, 0, 3, 5),
        };

        foreach (var (product, qty, warehouse, supplier, daysAgo) in receiptPlan)
        {
            var p = products[product];
            var date = today.AddDays(-daysAgo);
            receipts.Add(new GoodsReceipt
            {
                ProductId = p.ProductId,
                Quantity = qty,
                Unit = p.BaseUnit,
                UnitPrice = PriceFor(p.Name, qty),
                TaxRate = 9,
                InvoiceNumber = $"IR-{++receiptNumber}",
                InvoiceDate = date.AddDays(-2),
                ReceiptDate = date,
                SupplierId = suppliers[supplier].SupplierId,
                WarehouseId = warehouse == 0 ? central.WarehouseId : branch.WarehouseId,
                BatchNumber = $"B{today.Year}{daysAgo:000}",
                ShelfLocation = $"قفسه {(product % 6) + 1}-{(product % 4) + 1}",
                Status = 1,                       // تأییدشده؛ فقط این‌ها موجودی را جابه‌جا می‌کنند
                CreatedAt = date,
                UserId = userId,
                UserFullName = userName,
                ApprovedByUserId = userId,
                ApprovedByFullName = userName
            });
        }

        var issueNumber = 5000;
        // (شاخص کالا، مقدار، شاخص انبار، شاخص مشتری، نوع، چند روز پیش)
        (int Product, int Qty, int Warehouse, int Customer, IssueType Type, int DaysAgo)[] issuePlan =
        {
            (0,  120, 0, 0, IssueType.Sale,        80),
            (1,  200, 0, 0, IssueType.Sale,        74),
            (3,  300, 0, 2, IssueType.Sale,        60),
            (5,   80, 0, 0, IssueType.Sale,        52),
            (7,  100, 0, 1, IssueType.Sale,        44),
            (10, 150, 0, 1, IssueType.Sale,        38),
            (11, 250, 0, 0, IssueType.Sale,        32),
            (1,  150, 0, 2, IssueType.Sale,        26),
            (4,   60, 0, 0, IssueType.Sale,        22),
            (0,   90, 0, 1, IssueType.Sale,        16),
            (6,   40, 0, 0, IssueType.InternalUse, 14),
            (2,   70, 0, 1, IssueType.Sale,        10),
            (9,   25, 0, 0, IssueType.Sale,         8),
            (5,   45, 0, 2, IssueType.Sale,         6),
            (8,   30, 0, 1, IssueType.Sale,         4),
            (3,  180, 0, 0, IssueType.Sale,         2),
        };

        foreach (var (product, qty, warehouse, customer, type, daysAgo) in issuePlan)
        {
            var p = products[product];
            var date = today.AddDays(-daysAgo);
            issues.Add(new GoodsIssue
            {
                ProductId = p.ProductId,
                Quantity = qty,
                Unit = p.BaseUnit,
                UnitSellingPrice = PriceFor(p.Name, qty) * 1.15m,
                Type = type,
                CustomerId = customers[customer].CustomerId,
                WarehouseId = warehouse == 0 ? central.WarehouseId : branch.WarehouseId,
                InvoiceNumber = $"IV-{++issueNumber}",
                InvoiceDate = date.AddDays(-1),
                IssueDate = date,
                BatchNumber = $"B{today.Year}{(80 - daysAgo):000}",
                ShelfLocation = $"قفسه {(product % 6) + 1}-{(product % 4) + 1}",
                Status = 1,                       // تأییدشده
                CreatedAt = date,
                UserId = userId,
                UserFullName = userName,
                ApprovedByUserId = userId,
                ApprovedByFullName = userName
            });
        }

        // دو سند در وضعیت «در انتظار تأیید» تا دکمه‌های تأیید در دمو کار کنند.
        var pendingProduct = products[1];
        var pendingDate = today.AddDays(-1);
        receipts.Add(new GoodsReceipt
        {
            ProductId = pendingProduct.ProductId,
            Quantity = 250,
            Unit = pendingProduct.BaseUnit,
            UnitPrice = PriceFor(pendingProduct.Name, 250),
            TaxRate = 9,
            InvoiceNumber = $"IR-{++receiptNumber}",
            InvoiceDate = pendingDate,
            ReceiptDate = pendingDate,
            SupplierId = suppliers[0].SupplierId,
            WarehouseId = central.WarehouseId,
            BatchNumber = $"B{today.Year}999",
            Status = 0,
            CreatedAt = pendingDate,
            UserId = userId,
            UserFullName = userName
        });
        issues.Add(new GoodsIssue
        {
            ProductId = pendingProduct.ProductId,
            Quantity = 60,
            Unit = pendingProduct.BaseUnit,
            UnitSellingPrice = PriceFor(pendingProduct.Name, 60) * 1.15m,
            Type = IssueType.Sale,
            CustomerId = customers[0].CustomerId,
            WarehouseId = central.WarehouseId,
            InvoiceNumber = $"IV-{++issueNumber}",
            InvoiceDate = pendingDate,
            IssueDate = pendingDate,
            BatchNumber = $"B{today.Year}999",
            Status = 0,
            CreatedAt = pendingDate,
            UserId = userId,
            UserFullName = userName
        });

        context.GoodsReceipts.AddRange(receipts);
        context.GoodsIssues.AddRange(issues);
        await context.SaveChangesAsync();

        // ---------- موجودی نهایی ----------
        // همان فرمولی که سرویس‌ها هنگام تأیید اجرا می‌کنند، تا داشبورد و
        // هشدار «موجودی رو به اتمام» با اسناد هم‌خوان باشد.
        var approvedReceipts = await context.GoodsReceipts
            .Where(r => r.Status == 1)
            .Select(r => new { r.ProductId, r.Quantity, r.WarehouseId })
            .ToListAsync();
        var approvedIssues = await context.GoodsIssues
            .Where(i => i.Status == 1)
            .Select(i => new { i.ProductId, i.Quantity, i.WarehouseId })
            .ToListAsync();

        foreach (var p in products)
        {
            p.Number = approvedReceipts.Where(r => r.ProductId == p.ProductId).Sum(r => r.Quantity)
                    - approvedIssues.Where(i => i.ProductId == p.ProductId).Sum(i => i.Quantity);
        }
        await context.SaveChangesAsync();

        central.Number = approvedReceipts.Where(r => r.WarehouseId == central.WarehouseId).Sum(r => r.Quantity)
                        - approvedIssues.Where(i => i.WarehouseId == central.WarehouseId).Sum(i => i.Quantity);
        branch.Number = 0;   // دمو فعلاً همه‌چیز در انبار مرکزی است
        await context.SaveChangesAsync();
    }

    private static Product New(
        string name, int? categoryId, string unit, string barcode, int min, int max)
        => new()
        {
            Name = name,
            CategoryId = categoryId,
            BaseUnit = unit,
            Barcode = barcode,
            MinStockLevel = min,
            MaxStockLevel = max,
            IsActive = true
        };

    /// <summary>قیمت واحد ساختگی ولی واقع‌نما، متناسب با نوع کالا.</summary>
    private static decimal PriceFor(string productName, int quantity)
    {
        if (productName.Contains("برنج")) return 185_000m;
        if (productName.Contains("روغن")) return 320_000m;
        if (productName.Contains("پودر لباسشویی")) return 425_000m;
        if (productName.Contains("دستمال")) return 165_000m;
        if (productName.Contains("فیلتر")) return 890_000m;
        if (productName.Contains("لنت")) return 1_450_000m;
        if (productName.Contains("شوینده")) return 145_000m;
        if (productName.Contains("آب پرتقال")) return 78_000m;
        if (productName.Contains("نوشابه")) return 32_000m;
        if (productName.Contains("آب معدنی")) return 18_500m;
        if (productName.Contains("دفترچه")) return 96_000m;
        if (productName.Contains("خودکار")) return 22_000m;
        return 50_000m;
    }
}
