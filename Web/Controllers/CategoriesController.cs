using Application.Features.Definition.Category_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories) => _categories = categories;

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _categories.GetAllCategoriesAsync()
            : await _categories.SearchByNameAsync(keyword.Trim());

        var all = (await _categories.GetAllCategoriesAsync()).ToList();

        ViewData["Keyword"] = keyword;
        ViewBag.ParentNames = all.ToDictionary(c => c.CategoryId, c => c.Name);
        ViewBag.ChildCounts = all.ToDictionary(c => c.CategoryId, c => c.SubCategories.Count);
        return View(list.OrderBy(c => c.Name).ToList());
    }

    public async Task<IActionResult> Create()
    {
        await FillParentListAsync();
        return View("Form", new CategoryFormModel());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categories.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        await FillParentListAsync();

        return View("Form", new CategoryFormModel
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            ParentId = category.ParentId,
            Description = category.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillParentListAsync();
            return View("Form", model);
        }

        var category = ToEntity(model);
        var (success, message, _) = await _categories.CreateCategoryAsync(category);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            await FillParentListAsync();
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormModel model)
    {
        if (id != model.CategoryId) return NotFound();

        if (!ModelState.IsValid)
        {
            await FillParentListAsync();
            return View("Form", model);
        }

        var existing = await _categories.GetCategoryByIdAsync(id);
        if (existing == null) return NotFound();

        var category = ToEntity(model);
        category.IsActive = existing.IsActive;
        category.CreatedAt = existing.CreatedAt;

        var (success, message, _) = await _categories.UpdateCategoryAsync(category);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            await FillParentListAsync();
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var category = await _categories.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        var (success, message) = category.IsActive
            ? await _categories.DeactivateCategoryAsync(id)
            : await _categories.ActivateCategoryAsync(id);

        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _categories.DeleteCategoryAsync(id);
        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    private static Category ToEntity(CategoryFormModel model) => new()
    {
        CategoryId = model.CategoryId,
        Name = model.Name.Trim(),
        ParentId = model.ParentId,
        Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
    };

    private async Task FillParentListAsync()
    {
        var all = (await _categories.GetAllCategoriesAsync()).OrderBy(c => c.Name).ToList();
        ViewBag.Parents = all;
    }
}
