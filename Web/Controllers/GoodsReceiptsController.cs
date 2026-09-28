using Application.Dto;
using Application.Features.Definition.Customer_Service;
using Application.Features.Definition.GoodsIssue_Service;
using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Definition.Product_Service;
using Application.Features.Definition.Supplier_Service;
using Application.Features.Definition.Usermanagement_Service;
using Application.Features.Definition.Warehouse_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;
using Domain.Entity;

namespace Web.Controllers;

[Authorize]
public class GoodsReceiptsController : Controller
{
    private readonly IGoodsReceiptService _receipts;
    private readonly IGoodsIssueService _issues;
    private readonly IUserManagement _users;
    private readonly IProductService _products;
    private readonly IWarehouseService _warehouses;
    private readonly ISupplierService _suppliers;
    private readonly ICustomerService _customers;
    private readonly IReceiptValidator _validator;
    private readonly IClock _clock;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoodsReceiptsController(
        IGoodsReceiptService receipts,
        IGoodsIssueService issues,
        IUserManagement users,
        IProductService products,
        IWarehouseService warehouses,
        ISupplierService suppliers,
        ICustomerService customers,
        IReceiptValidator validator,
        IClock clock,
        UserManager<ApplicationUser> userManager)
    {
        _receipts = receipts;
        _issues = issues;
        _users = users;
        _products = products;
        _warehouses = warehouses;
        _suppliers = suppliers;
        _customers = customers;
        _validator = validator;
        _clock = clock;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _receipts.GetTopReceiptsWithDetailsAsync(200)
            : await _receipts.SearchReceiptsAsync(keyword, 200);

        ViewData["Keyword"] = keyword;
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var receipt = await _receipts.GetReceiptWithDetailsAsync(id);
        if (receipt == null)
            return NotFound();

        return View(receipt);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? productId)
    {
        await FillFormLookupsAsync();
        return View("Form", new ReceiptFormModel
        {
            ProductId = productId,
            InvoiceDate = _clock.Today,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReceiptFormModel model)
    {
        RequireSupplier(model);

        if (!ModelState.IsValid)
        {
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var product = await ResolveProductAsync(model.ProductId);
        if (product is null)
        {
            ModelState.AddModelError(nameof(model.ProductId), "کالای انتخاب‌شده معتبر نیست.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        if (model.InvoiceDate.Date > _clock.Today)
        {
            ModelState.AddModelError(nameof(model.InvoiceDate), "تاریخ فاکتور نمی‌تواند در آینده باشد.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var check = _validator.Validate(
            model.InvoiceNumber,
            model.InvoiceDate.ToString("yyyy-MM-dd"),
            model.Quantity,
            model.UnitPrice,
            model.TaxRate,
            model.SupplierId ?? 0,
            model.WarehouseId,
            product.Number ?? 0);

        if (!check.IsValid)
        {
            // خطای تأمین‌کننده کلید خالی دارد و فقط در خلاصه‌ی بالای فرم دیده می‌شود؛
            // آن را به فیلد خودش می‌چسبانیم تا کنار همان فیلد نمایش داده شود.
            var key = model.SupplierId is null or <= 0
                ? nameof(model.SupplierId)
                : string.Empty;
            ModelState.AddModelError(key, check.Error!);
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var user = await _userManager.GetUserAsync(User);

        var receipt = new GoodsReceipt
        {
            ProductId = product.ProductId,
            Quantity = model.Quantity,
            UnitPrice = model.UnitPrice,
            TaxRate = model.TaxRate,
            Unit = model.Unit.Trim(),
            WarehouseId = model.WarehouseId,
            SupplierId = model.SupplierId!.Value,
            InvoiceNumber = model.InvoiceNumber.Trim(),
            InvoiceDate = model.InvoiceDate.Date,
            ScannedBarcode = NullIfBlank(model.ScannedBarcode),
            ShelfLocation = NullIfBlank(model.ShelfLocation),
            BatchNumber = NullIfBlank(model.BatchNumber),
            Description = NullIfBlank(model.Description),
            ReceiptDate = _clock.Today,
            CreatedAt = _clock.Now,
            Status = 0,
            UserId = user?.Id,
            UserFullName = user?.FullName ?? user?.UserName,
        };

        await _receipts.AddAsync(receipt);

        TempData["Message"] = check.Warning is null
            ? "رسید انبار ثبت شد و در انتظار تأیید است."
            : $"رسید انبار ثبت شد. هشدار: {check.Warning}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var receipt = await _receipts.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        // رسید تأییدشده یا لغوشده قابل ویرایش نیست: موجودی انبار بر اساس مقدار
        // همان لحظه تأیید تغییر کرده و ویرایش مقدار، موجودی را با سند ناهماهنگ می‌کند.
        if (receipt.Status != 0)
        {
            TempData["Error"] = "فقط رسیدهای در انتظار تأیید قابل ویرایش هستند.";
            return RedirectToAction(nameof(Index));
        }

        await FillFormLookupsAsync();

        return View("Form", new ReceiptFormModel
        {
            ReceiptId = receipt.ReceiptId,
            ProductId = receipt.ProductId,
            Quantity = receipt.Quantity,
            UnitPrice = receipt.UnitPrice,
            TaxRate = receipt.TaxRate,
            Unit = receipt.Unit,
            WarehouseId = receipt.WarehouseId,
            SupplierId = receipt.SupplierId,
            InvoiceNumber = receipt.InvoiceNumber,
            InvoiceDate = receipt.InvoiceDate,
            ScannedBarcode = receipt.ScannedBarcode,
            ShelfLocation = receipt.ShelfLocation,
            BatchNumber = receipt.BatchNumber,
            Description = receipt.Description,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReceiptFormModel model)
    {
        var receipt = await _receipts.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        if (receipt.Status != 0)
        {
            TempData["Error"] = "فقط رسیدهای در انتظار تأیید قابل ویرایش هستند.";
            return RedirectToAction(nameof(Index));
        }

        model.ReceiptId = id;

        RequireSupplier(model);

        if (!ModelState.IsValid)
        {
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        if (model.InvoiceDate.Date > _clock.Today)
        {
            ModelState.AddModelError(nameof(model.InvoiceDate), "تاریخ فاکتور نمی‌تواند در آینده باشد.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var product = await ResolveProductAsync(model.ProductId);
        if (product is null)
        {
            ModelState.AddModelError(nameof(model.ProductId), "کالای انتخاب‌شده معتبر نیست.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var check = _validator.Validate(
            model.InvoiceNumber,
            model.InvoiceDate.ToString("yyyy-MM-dd"),
            model.Quantity,
            model.UnitPrice,
            model.TaxRate,
            model.SupplierId ?? 0,
            model.WarehouseId,
            product.Number ?? 0);

        if (!check.IsValid)
        {
            var key = model.SupplierId is null or <= 0
                ? nameof(model.SupplierId)
                : string.Empty;
            ModelState.AddModelError(key, check.Error!);
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        // کالا در ویرایش تغییر نمی‌کند (مانند نسخه دسکتاپ): رسید به یک کالای مشخص تعلق دارد.
        receipt.Quantity = model.Quantity;
        receipt.UnitPrice = model.UnitPrice;
        receipt.TaxRate = model.TaxRate;
        receipt.Unit = model.Unit.Trim();
        receipt.WarehouseId = model.WarehouseId;
        receipt.SupplierId = model.SupplierId!.Value;
        receipt.InvoiceNumber = model.InvoiceNumber.Trim();
        receipt.InvoiceDate = model.InvoiceDate.Date;
        receipt.ScannedBarcode = NullIfBlank(model.ScannedBarcode);
        receipt.ShelfLocation = NullIfBlank(model.ShelfLocation);
        receipt.BatchNumber = NullIfBlank(model.BatchNumber);
        receipt.Description = NullIfBlank(model.Description);

        await _receipts.UpdateAsync(receipt);

        TempData["Message"] = check.Warning is null
            ? "رسید انبار ویرایش شد."
            : $"رسید انبار ویرایش شد. هشدار: {check.Warning}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanApprove)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? returnUrl)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id ?? string.Empty;

        var (success, message) = await _receipts.ApproveReceiptAsync(id, userId);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success, message });

        TempData["Message"] = message;
        return Redirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action(nameof(Index)));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanApprove)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? returnUrl)
    {
        var ok = await _receipts.CancelReceiptAsync(id);

        TempData["Message"] = ok ? "رسید لغو شد." : "لغو رسید ناموفق بود.";
        return Redirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action(nameof(Index)));
    }

    /// <summary>
    /// جستجوی کالا با بارکد برای بارکدخوان. بارکدخوان مثل صفحه‌کلید تایپ می‌کند و
    /// Enter می‌فرستد، پس این نقطه پایان همان تجربه کاربر دسکتاپ است.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> BarcodeLookup(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return Json(new { found = false, message = "بارکد وارد نشده است." });

        var product = await _products.GetByBarcodeAsync(barcode.Trim());
        if (product == null)
            return Json(new { found = false, message = "کالایی با این بارکد یافت نشد." });

        if (!product.IsActive)
            return Json(new { found = false, message = "این کالا غیرفعال است و امکان ثبت تراکنش ندارد." });

        return Json(new
        {
            found = true,
            productId = product.ProductId,
            name = product.Name,
            baseUnit = product.BaseUnit,
            stock = product.Number,
            message = $"{product.Name} انتخاب شد.",
        });
    }

    private async Task<Product?> ResolveProductAsync(Guid? productId)
    {
        if (productId is null || productId == Guid.Empty)
            return null;

        var product = await _products.GetProductByIdAsync(productId.Value);
        return product is { IsActive: true } ? product : null;
    }

    /// <summary>
    /// هر رسیدی به یک تأمین‌کننده نیاز دارد. این بررسی جدا از اعتبارسنج سرویس
    /// انجام می‌شود تا پیام کنار خود فیلد بنشیند؛ سرویس این خطا را با کلید خالی
    /// برمی‌گرداند و نتیجه‌اش فقط دیده‌شدن در خلاصه‌ی بالای فرم بود.
    /// </summary>
    private void RequireSupplier(ReceiptFormModel model)
    {
        if (model.SupplierId is null or <= 0)
            ModelState.AddModelError(nameof(model.SupplierId), "تأمین‌کننده را انتخاب کنید.");
    }

    private async Task FillFormLookupsAsync()
    {
        var products = await _products.GetAllProductsAsync();
        ViewBag.Products = products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new ProductLookupItem
            {
                Value = p.ProductId,
                Text = p.Name,
                BaseUnit = p.BaseUnit,
                Barcode = p.Barcode,
                Stock = p.Number,
            })
            .ToList();

        ViewBag.Warehouses = (await _warehouses.GetAllAsync())
            .Where(w => !string.Equals(w.Status, "غیرفعال", StringComparison.OrdinalIgnoreCase))
            .OrderBy(w => w.Name)
            .Select(w => new LookupItem { Value = w.WarehouseId, Text = w.Name })
            .ToList();

        ViewBag.Suppliers = (await _suppliers.GetAllAsync())
            .OrderBy(s => s.Name)
            .Select(s => new LookupItem { Value = s.SupplierId, Text = s.Name })
            .ToList();

        ViewBag.Customers = (await _customers.GetAllAsync())
            .OrderBy(c => c.Name)
            .Select(c => new LookupItem { Value = c.CustomerId, Text = c.Name })
            .ToList();
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
