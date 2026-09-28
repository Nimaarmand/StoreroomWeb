using Application.Features.Definition.Warehouse_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class WarehousesController : Controller
{
    private readonly IWarehouseService _warehouses;

    public WarehousesController(IWarehouseService warehouses) => _warehouses = warehouses;

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _warehouses.GetAllAsync()
            : await _warehouses.SearchAsync(keyword.Trim());

        ViewData["Keyword"] = keyword;
        return View(list.OrderBy(w => w.Name).ToList());
    }

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public IActionResult Create() => View("Form", new WarehouseFormModel());

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public async Task<IActionResult> Edit(int id)
    {
        var warehouse = await _warehouses.GetByIdAsync(id);
        if (warehouse == null) return NotFound();

        return View("Form", new WarehouseFormModel
        {
            WarehouseId = warehouse.WarehouseId,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Min = warehouse.Min,
            Max = warehouse.Max,
            Status = warehouse.Status
        });
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WarehouseFormModel model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        if (model.Min > model.Max)
        {
            ModelState.AddModelError(nameof(model.Max), "حداکثر ظرفیت نمی‌تواند از حداقل ظرفیت کمتر باشد.");
            return View("Form", model);
        }

        if (!await _warehouses.IsWarehouseNameUniqueAsync(model.Name.Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "انباری با این نام قبلاً ثبت شده است.");
            return View("Form", model);
        }

        var warehouse = ToEntity(model);
        var created = await _warehouses.AddAsync(warehouse);

        TempData["Message"] = $"انبار «{created.Name}» ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WarehouseFormModel model)
    {
        if (id != model.WarehouseId) return NotFound();

        if (!ModelState.IsValid) return View("Form", model);

        if (model.Min > model.Max)
        {
            ModelState.AddModelError(nameof(model.Max), "حداکثر ظرفیت نمی‌تواند از حداقل ظرفیت کمتر باشد.");
            return View("Form", model);
        }

        if (!await _warehouses.IsWarehouseNameUniqueAsync(model.Name.Trim(), id))
        {
            ModelState.AddModelError(nameof(model.Name), "انباری با این نام قبلاً ثبت شده است.");
            return View("Form", model);
        }

        var warehouse = ToEntity(model);
        var updated = await _warehouses.UpdateAsync(warehouse);   // موجودی در سرویس حفظ می‌شود
        TempData["Message"] = $"انبار «{updated.Name}» بروزرسانی شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _warehouses.DeleteAsync(id);

        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    private static Warehouse ToEntity(WarehouseFormModel model) => new()
    {
        WarehouseId = model.WarehouseId,
        Name = model.Name.Trim(),
        Location = string.IsNullOrWhiteSpace(model.Location) ? null : model.Location.Trim(),
        Min = model.Min,
        Max = model.Max,
        Status = string.IsNullOrWhiteSpace(model.Status) ? "فعال" : model.Status.Trim()
    };
}
