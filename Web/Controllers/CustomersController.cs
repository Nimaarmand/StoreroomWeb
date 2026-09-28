using Application.Features.Definition.Customer_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customers;

    public CustomersController(ICustomerService customers) => _customers = customers;

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _customers.GetAllAsync()
            : await _customers.SearchByNameAsync(keyword.Trim());

        ViewData["Keyword"] = keyword;
        return View(list.OrderBy(c => c.Name).ToList());
    }

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public IActionResult Create() => View("Form", new CustomerFormModel());

    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customers.GetByIdAsync(id);
        if (customer == null) return NotFound();

        return View("Form", new CustomerFormModel
        {
            CustomerId = customer.CustomerId,
            Name = customer.Name,
            Phone = customer.Phone,
            MarketName = customer.MarketName,
            Address = customer.Address
        });
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerFormModel model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        var (success, message, _) = await _customers.CreateCustomerAsync(ToEntity(model));

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CanEditMasterData)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerFormModel model)
    {
        if (id != model.CustomerId) return NotFound();

        if (!ModelState.IsValid) return View("Form", model);

        // سرویس تاریخ ایجاد را در ویرایش از رکورد موجودی بازیابی می‌کند،
        // پس نیازی به دست‌کاری آن اینجا نیست.
        var (success, message, _) = await _customers.UpdateCustomerAsync(ToEntity(model));

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View("Form", model);
        }

        TempData["Message"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _customers.DeleteCustomerAsync(id);
        if (success) TempData["Message"] = message;
        else TempData["Error"] = message;

        return RedirectToAction(nameof(Index));
    }

    private static Customer ToEntity(CustomerFormModel model) => new()
    {
        CustomerId = model.CustomerId,
        Name = model.Name.Trim(),
        Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
        MarketName = string.IsNullOrWhiteSpace(model.MarketName) ? null : model.MarketName.Trim(),
        Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim()
    };
}
