using Application.Dto;
using Application.Features.Definition.GoodsIssue_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;

namespace Web.Controllers;

[Authorize]
public class GoodsIssuesController : Controller
{
    private readonly IGoodsIssueService _issues;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoodsIssuesController(IGoodsIssueService issues, UserManager<ApplicationUser> userManager)
    {
        _issues = issues;
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
}
