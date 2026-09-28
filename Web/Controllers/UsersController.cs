using Application.Features.Definition.Usermanagement_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

/// <summary>
/// مدیریت کاربران و نقش‌ها. کل کنترلر AdminOnly است چون ساخت یا حذف کاربر
/// دسترسی گسترده‌ای به کل سامانه می‌دهد.
/// </summary>
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class UsersController : Controller
{
    private readonly IUserManagement _users;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(IUserManagement users, UserManager<ApplicationUser> userManager)
    {
        _users = users;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var list = string.IsNullOrWhiteSpace(keyword)
            ? await _users.GetAllUsersAsync()
            : await _users.SearchUsersAsync(keyword.Trim());

        var rows = new List<UserListItem>();
        foreach (var u in list)
        {
            rows.Add(new UserListItem
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                Roles = (await _users.GetUserRolesAsync(u)).ToList()
            });
        }

        ViewData["Keyword"] = keyword;
        return View(rows.OrderBy(r => r.FullName).ToList());
    }

    public async Task<IActionResult> Create()
    {
        var model = new UserFormModel
        {
            AvailableRoles = (await _users.GetAllRolesAsync()).Select(r => r.Name!).ToList()
        };
        return View("Form", model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _users.FindUserByIdAsync(id);
        if (user is null) return NotFound();

        var model = new UserFormModel
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Roles = (await _users.GetUserRolesAsync(user)).ToList(),
            AvailableRoles = (await _users.GetAllRolesAsync()).Select(r => r.Name!).ToList()
        };

        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormModel model)
    {
        await FillRolesAsync(model);

        if (!ModelState.IsValid) return View("Form", model);

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            FullName = model.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim(),
            IsActive = model.IsActive
        };

        var result = await _users.RegisterUserAsync(user, model.Password!, UserFormModel.DefaultRole);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View("Form", model);
        }

        TempData["Message"] = $"کاربر «{user.UserName}» ساخته شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserFormModel model)
    {
        model.Id = id;
        await FillRolesAsync(model);

        // رمز هنگام ویرایش اختیاری است: خالی ماندن یعنی «تغییر نده».
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            model.Password = null;
            model.ConfirmPassword = null;
            ModelState.Remove(nameof(model.Password));
            ModelState.Remove(nameof(model.ConfirmPassword));
        }
        if (!ModelState.IsValid) return View("Form", model);

        var user = await _users.FindUserByIdAsync(id);
        if (user is null) return NotFound();

        // نام کاربری از فرم قابل تغییر نیست، پس مقدار ارسالی نادیده گرفته و
        // مقدار واقعی از دیتابیس جایگزین می‌شود.
        model.UserName = user.UserName ?? string.Empty;
        user.FullName = model.FullName.Trim();
        user.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();
        user.IsActive = model.IsActive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View("Form", model);
        }

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            // ChangePasswordAsync رمز فعلی را می‌خواهد که مدیر آن را ندارد،
            // پس توکن ریست می‌گیریم و مستقیم می‌گذاریم.
            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var reset = await _users.ResetPasswordAsync(user, token, model.Password);
            if (!reset.Succeeded)
            {
                AddErrors(reset);
                return View("Form", model);
            }
        }

        await SyncRolesAsync(user, model.Roles);

        TempData["Message"] = $"کاربر «{user.UserName}» به‌روزرسانی شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _users.FindUserByIdAsync(id);
        if (user is null) return NotFound();

        // خود مدیر فعال را نباید غیرفعال کند، وگرنه ممکن است سامانه
        // بدون مدیر بماند.
        var self = await _userManager.GetUserAsync(User);
        if (self is not null && self.Id == user.Id)
        {
            TempData["Error"] = "نمی‌توانید حساب خودتان را غیرفعال کنید.";
            return RedirectToAction(nameof(Index));
        }

        var result = user.IsActive
            ? await _users.DeactivateUserAsync(id)
            : await _users.ActivateUserAsync(id);

        TempData[result.Succeeded ? "Message" : "Error"] = result.Succeeded
            ? (user.IsActive ? "کاربر غیرفعال شد." : "کاربر فعال شد.")
            : string.Join("، ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _users.FindUserByIdAsync(id);
        if (user is null) return NotFound();

        var self = await _userManager.GetUserAsync(User);
        if (self is not null && self.Id == user.Id)
        {
            TempData["Error"] = "نمی‌توانید حساب خودتان را حذف کنید.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _users.DeleteUserAsync(user);
        TempData[result.Succeeded ? "Message" : "Error"] = result.Succeeded
            ? $"کاربر «{user.UserName}» حذف شد."
            : string.Join("، ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// نقش‌های انتخابی را با نقش‌های واقعی کاربر هم‌تراز می‌کند.
    /// خالی بودن فهرست یعنی «هیچ نقشی» و همه نقش‌ها برداشته می‌شوند.
    /// </summary>
    private async Task SyncRolesAsync(ApplicationUser user, List<string> desired)
    {
        var current = (await _users.GetUserRolesAsync(user)).ToList();
        desired = desired.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();

        foreach (var role in current.Except(desired))
            await _users.RemoveFromRoleAsync(user, role);

        foreach (var role in desired.Except(current))
            await _users.AddToRoleAsync(user, role);
    }

    private async Task FillRolesAsync(UserFormModel model)
    {
        var all = (await _users.GetAllRolesAsync()).Select(r => r.Name!).ToList();

        // چک‌باکس‌های نقش تک‌مقداری می‌فرستند، پس باید از آرایه خام خوانده شوند.
        // اگر هیچ چک‌باکسی تیک نخورده باشد، آرایه خالی یا null است.
        model.Roles = model.RolesSubmitted?.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList()
                      ?? new List<string>();

        // اگر نقشی در فرم انتخاب شده که در سامانه وجود ندارد، همان‌جا خطا بده
        // تا مدیر فکر نکند نقش اعمال شده است.
        foreach (var role in model.Roles.Where(r => !all.Contains(r)))
            ModelState.AddModelError(nameof(model.Roles), $"نقش «{role}» وجود ندارد.");

        model.AvailableRoles = all;
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }
}
