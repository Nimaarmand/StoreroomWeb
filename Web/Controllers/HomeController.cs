using Application.Features.Definition.Product_Service;
using Application.Features.Definition.Warehouse_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProductService _products;
    private readonly IWarehouseService _warehouses;

    public HomeController(IProductService products, IWarehouseService warehouses)
    {
        _products = products;
        _warehouses = warehouses;
    }

    public async Task<IActionResult> Index()
    {
        var products = (await _products.GetAllProductsAsync()).ToList();
        var warehouses = (await _warehouses.GetAllAsync()).ToList();

        var model = new DashboardViewModel
        {
            ProductCount = products.Count,
            ActiveProductCount = products.Count(p => p.IsActive),
            WarehouseCount = warehouses.Count,
            TotalStock = products.Sum(p => p.Number ?? 0),
            LowStockProducts = products
                .Where(p => p.IsActive && p.Number.HasValue)
                .Select(p => new DashboardViewModel.LowStockRow
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Unit = p.BaseUnit,
                    Stock = p.Number!.Value,
                    MinStockLevel = p.MinStockLevel,
                })
                .Where(r => r.MinStockLevel.HasValue && r.Stock <= r.MinStockLevel.Value)
                .OrderBy(r => r.Stock)
                .Take(10)
                .ToList(),
            WarehouseStocks = warehouses
                .Select(w => new DashboardViewModel.WarehouseStockRow
                {
                    Name = w.Name,
                    Location = w.Location,
                    Number = w.Number,
                    Min = w.Min,
                    Max = w.Max,
                })
                .OrderByDescending(w => w.Number)
                .ToList(),
        };

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Error(string? message)
    {
        ViewData["Title"] = "خطا";
        ViewData["Message"] = message;
        return View();
    }
}
