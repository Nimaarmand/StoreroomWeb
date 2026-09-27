using Application.Dto;
using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Definition.Usermanagement_Service;
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
    private readonly IUserManagement _users;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoodsReceiptsController(
        IGoodsReceiptService receipts,
        IUserManagement users,
        UserManager<ApplicationUser> userManager)
    {
        _receipts = receipts;
        _users = users;
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
}
