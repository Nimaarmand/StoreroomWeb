using Application.Dto;
using Application.Features.Definition.Customer_Service;
using Application.Features.Definition.GoodsIssue_Service;
using Application.Features.Definition.Product_Service;
using Application.Features.Definition.Warehouse_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class GoodsIssuesController : Controller
{
    private readonly IGoodsIssueService _issues;
    private readonly IProductService _products;
    private readonly IWarehouseService _warehouses;
    private readonly ICustomerService _customers;
    private readonly IIssueValidator _validator;
    private readonly IClock _clock;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoodsIssuesController(
        IGoodsIssueService issues,
        IProductService products,
        IWarehouseService warehouses,
        ICustomerService customers,
        IIssueValidator validator,
        IClock clock,
        UserManager<ApplicationUser> userManager)
    {
        _issues = issues;
        _products = products;
        _warehouses = warehouses;
        _customers = customers;
        _validator = validator;
        _clock = clock;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _issues.GetTopIssuesWithDetailsAsync(200)
            : await _issues.SearchIssuesAsync(keyword, 200);

        ViewData["Keyword"] = keyword;
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var issue = await _issues.GetIssueWithDetailsAsync(id);
        if (issue == null)
            return NotFound();

        return View(issue);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? productId)
    {
        await FillFormLookupsAsync();
        return View("Form", new IssueFormModel
        {
            ProductId = productId,
            Type = IssueType.Sale,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IssueFormModel model)
    {
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

        // مشتری فقط برای فروش معنا دارد. برای ضایعات یا انتقال، مشتری نداشتن
        // درست است و اجباری کردنش کاربر را مجبور به انتخاب داده بی‌ربط می‌کرد.
        if (model.Type == IssueType.Sale && model.CustomerId is null)
        {
            ModelState.AddModelError(nameof(model.CustomerId), "برای حواله فروش، مشتری را انتخاب کنید.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var check = _validator.Validate(
            model.Unit,
            model.Quantity,
            product.ProductId,
            model.WarehouseId,
            model.Type == IssueType.Sale ? model.CustomerId : null,
            product.Number);

        if (!check.IsValid)
        {
            var key = check.Error!.Contains("موجودی")
                ? nameof(model.Quantity)
                : string.Empty;
            ModelState.AddModelError(key, check.Error!);
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var user = await _userManager.GetUserAsync(User);

        var issue = new GoodsIssue
        {
            ProductId = product.ProductId,
            Quantity = model.Quantity,
            Unit = model.Unit.Trim(),
            UnitSellingPrice = model.UnitSellingPrice,
            Type = model.Type,
            CustomerId = model.Type == IssueType.Sale ? model.CustomerId : null,
            InvoiceNumber = NullIfBlank(model.InvoiceNumber),
            WarehouseId = model.WarehouseId,
            Description = NullIfBlank(model.Description),
            IssueDate = _clock.Today,
            CreatedAt = _clock.Now,
            Status = 0,
            UserId = user?.Id,
            UserFullName = user?.FullName ?? user?.UserName,
        };

        await _issues.AddAsync(issue);

        TempData["Message"] = check.Warning is null
            ? "حواله خروج ثبت شد و در انتظار تأیید است."
            : $"حواله خروج ثبت شد. هشدار: {check.Warning}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var issue = await _issues.GetByIdAsync(id);
        if (issue == null)
            return NotFound();

        // مانند رسید: سند تأییدشده یا لغوشده قابل ویرایش نیست، چون موجودی
        // انبار در لحظه تأیید بر اساس مقدار همان سند تغییر کرده است.
        if (issue.Status != 0)
        {
            TempData["Error"] = "فقط حواله‌های در انتظار تأیید قابل ویرایش هستند.";
            return RedirectToAction(nameof(Index));
        }

        await FillFormLookupsAsync();

        return View("Form", new IssueFormModel
        {
            IssueId = issue.IssueId,
            ProductId = issue.ProductId,
            Quantity = issue.Quantity,
            Unit = issue.Unit,
            UnitSellingPrice = issue.UnitSellingPrice,
            Type = issue.Type,
            CustomerId = issue.CustomerId,
            InvoiceNumber = issue.InvoiceNumber,
            WarehouseId = issue.WarehouseId,
            Description = issue.Description,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IssueFormModel model)
    {
        var issue = await _issues.GetByIdAsync(id);
        if (issue == null)
            return NotFound();

        if (issue.Status != 0)
        {
            TempData["Error"] = "فقط حواله‌های در انتظار تأیید قابل ویرایش هستند.";
            return RedirectToAction(nameof(Index));
        }

        model.IssueId = id;

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

        if (model.Type == IssueType.Sale && model.CustomerId is null)
        {
            ModelState.AddModelError(nameof(model.CustomerId), "برای حواله فروش، مشتری را انتخاب کنید.");
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        var check = _validator.Validate(
            model.Unit,
            model.Quantity,
            product.ProductId,
            model.WarehouseId,
            model.Type == IssueType.Sale ? model.CustomerId : null,
            product.Number);

        if (!check.IsValid)
        {
            // کمبود موجودی به فیلد مقدار مربوط است، پس پیام کنار همان فیلد
            // نمایش داده می‌شود نه فقط در خلاصه‌ی بالای فرم.
            var key = check.Error!.Contains("موجودی")
                ? nameof(model.Quantity)
                : string.Empty;
            ModelState.AddModelError(key, check.Error!);
            await FillFormLookupsAsync();
            return View("Form", model);
        }

        issue.Quantity = model.Quantity;
        issue.Unit = model.Unit.Trim();
        issue.UnitSellingPrice = model.UnitSellingPrice;
        issue.Type = model.Type;
        issue.CustomerId = model.Type == IssueType.Sale ? model.CustomerId : null;
        issue.InvoiceNumber = NullIfBlank(model.InvoiceNumber);
        issue.WarehouseId = model.WarehouseId;
        issue.Description = NullIfBlank(model.Description);

        await _issues.UpdateAsync(issue);

        TempData["Message"] = check.Warning is null
            ? "حواله خروج ویرایش شد."
            : $"حواله خروج ویرایش شد. هشدار: {check.Warning}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanApprove)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? returnUrl)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id ?? string.Empty;

        var (success, message) = await _issues.ApproveIssueAsync(id, userId);

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
        var ok = await _issues.CancelIssueAsync(id);

        TempData["Message"] = ok ? "حواله لغو شد." : "لغو حواله ناموفق بود.";
        return Redirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action(nameof(Index)));
    }

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

        ViewBag.Customers = (await _customers.GetAllAsync())
            .OrderBy(c => c.Name)
            .Select(c => new LookupItem { Value = c.CustomerId, Text = c.Name })
            .ToList();
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
