using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Definition.Supplier_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ISupplierService _suppliers;
    private readonly IGoodsReceiptService _receipts;

    public SuppliersController(ISupplierService suppliers, IGoodsReceiptService receipts)
    {
        _suppliers = suppliers;
        _receipts = receipts;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _suppliers.GetAllAsync()
            : await _suppliers.SearchAsync(keyword.Trim());

        ViewData["Keyword"] = keyword;
        return View(list.OrderBy(s => s.Name).ToList());
    }

    public IActionResult Create() => View("Form", new SupplierFormModel());

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _suppliers.GetByIdAsync(id);
        if (supplier == null) return NotFound();

        return View("Form", new SupplierFormModel
        {
            SupplierId = supplier.SupplierId,
            Name = supplier.Name,
            Phone = supplier.Phone,
            EconomicCode = supplier.EconomicCode,
            Address = supplier.Address
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierFormModel model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        var supplier = ToEntity(model);
        supplier.SupplierId = 0;   // تکلیف به دیتابیس

        if (!string.IsNullOrWhiteSpace(supplier.Phone) &&
            await _suppliers.IsPhoneDuplicateAsync(supplier.Phone))
        {
            ModelState.AddModelError(nameof(model.Phone), "این شماره تلفن قبلاً ثبت شده است.");
            return View("Form", model);
        }

        var created = await _suppliers.AddAsync(supplier);
        TempData["Message"] = $"تأمین‌کننده «{created.Name}» ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierFormModel model)
    {
        if (id != model.SupplierId) return NotFound();

        if (!ModelState.IsValid) return View("Form", model);

        var existing = await _suppliers.GetByIdAsync(id);
        if (existing == null) return NotFound();

        var supplier = ToEntity(model);

        if (!string.IsNullOrWhiteSpace(supplier.Phone) &&
            await _suppliers.IsPhoneDuplicateAsync(supplier.Phone, id))
        {
            ModelState.AddModelError(nameof(model.Phone), "این شماره تلفن قبلاً ثبت شده است.");
            return View("Form", model);
        }

        var updated = await _suppliers.UpdateAsync(supplier);
        TempData["Message"] = $"تأمین‌کننده «{updated.Name}» بروزرسانی شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _suppliers.GetByIdAsync(id);
        if (supplier == null) return NotFound();

        // تأمین‌کننده دارای رسید ورودی نباید حذف شود تا سوابق خرید حفظ گردد.
        var receipts = await _receipts.GetBySupplierIdAsync(id);
        if (receipts.Any())
        {
            TempData["Error"] = "این تأمین‌کننده رسید ورودی دارد و به دلیل حفظ سوابق مالی قابل حذف نیست.";
            return RedirectToAction(nameof(Index));
        }

        await _suppliers.RemoveAsync(supplier);
        TempData["Message"] = $"تأمین‌کننده «{supplier.Name}» حذف شد.";
        return RedirectToAction(nameof(Index));
    }

    private static Supplier ToEntity(SupplierFormModel model) => new()
    {
        SupplierId = model.SupplierId,
        Name = model.Name.Trim(),
        Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
        EconomicCode = string.IsNullOrWhiteSpace(model.EconomicCode) ? null : model.EconomicCode.Trim(),
        Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim()
    };
}
