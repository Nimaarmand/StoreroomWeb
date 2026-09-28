using Application.Features.Definition.Category_Service;
using Application.Features.Definition.Product_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _products;
    private readonly ICategoryService _categories;

    public ProductsController(IProductService products, ICategoryService categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<IActionResult> Index(string? keyword, bool lowStockOnly = false)
    {
        IEnumerable<Product> list;

        if (lowStockOnly)
        {
            list = await _products.GetLowStockProductsAsync();
        }
        else if (!string.IsNullOrWhiteSpace(keyword))
        {
            list = await _products.SearchByNameAsync(keyword.Trim());
        }
        else
        {
            list = await _products.GetAllProductsAsync();
        }

        var categories = await _categories.GetAllCategoriesAsync();
        ViewData["Keyword"] = keyword;
        ViewData["LowStockOnly"] = lowStockOnly;
        ViewBag.CategoryNames = categories.ToDictionary(c => c.CategoryId, c => c.Name);
        return View(list.OrderBy(p => p.Name).ToList());
    }

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public async Task<IActionResult> Create()
    {
        await FillCategoryListAsync();
        return View("Form", new ProductFormModel());
    }

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _products.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        await FillCategoryListAsync();

        return View("Form", new ProductFormModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            CategoryId = product.CategoryId,
            BaseUnit = product.BaseUnit,
            Barcode = product.Barcode,
            Number = product.Number,
            MinStockLevel = product.MinStockLevel,
            MaxStockLevel = product.MaxStockLevel,
            Weight = product.Weight,
            ProductionDate = product.ProductionDate,
            ExpirationDate = product.ExpirationDate,
            Description = product.Description
        });
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillCategoryListAsync();
            return View("Form", model);
        }

        var product = ToEntity(model);

        // موجودی اولیه فقط هنگام ایجاد معنا دارد؛ در ویرایش موجودی از
        // طریق رسید و حواله تغییر می‌کند و نباید از فرم بازنویسی شود.
        product.Number = model.Number ?? 0;

        var (success, message, _) = await _products.CreateProductAsync(product);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            await FillCategoryListAsync();
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductFormModel model)
    {
        if (id != model.ProductId) return NotFound();

        if (!ModelState.IsValid)
        {
            await FillCategoryListAsync();
            return View("Form", model);
        }

        var existing = await _products.GetProductByIdAsync(id);
        if (existing == null) return NotFound();

        var product = ToEntity(model);
        product.Number = existing.Number;   // حفظ موجودی واقعی
        product.IsActive = existing.IsActive;
        product.CreatedAt = existing.CreatedAt;

        var (success, message, _) = await _products.UpdateProductAsync(product);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            await FillCategoryListAsync();
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var product = await _products.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        var (success, message) = product.IsActive
            ? await _products.DeactivateProductAsync(id)
            : await _products.ActivateProductAsync(id);

        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (success, message) = await _products.DeleteProductAsync(id);
        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    private static Product ToEntity(ProductFormModel model) => new()
    {
        ProductId = model.ProductId,
        Name = model.Name.Trim(),
        CategoryId = model.CategoryId,
        BaseUnit = model.BaseUnit.Trim(),
        Barcode = string.IsNullOrWhiteSpace(model.Barcode) ? null : model.Barcode.Trim(),
        MinStockLevel = model.MinStockLevel,
        MaxStockLevel = model.MaxStockLevel,
        Weight = model.Weight,
        ProductionDate = model.ProductionDate,
        ExpirationDate = model.ExpirationDate,
        Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
    };

    private async Task FillCategoryListAsync()
    {
        var categories = await _categories.GetAllCategoriesAsync();
        ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
    }
}
