using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        // حساب غیرفعال باید نتواند وارد شود. این بررسی قبل از PasswordSignInAsync
        // انجام می‌شود تا نتیجه به کاربر لو نرود که رمز درست است یا نه، و در
        // نتیجه PasswordSignInAsync هم دوباره انجام نمی‌شود.
        var candidate = await _userManager.FindByNameAsync(model.UserName);
        if (candidate is { IsActive: false })
        {
            ModelState.AddModelError(string.Empty, "این حساب غیرفعال شده است. با مدیر سامانه تماس بگیرید.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // غیرفعال کردن کاربر باید نشست باز او را هم ببندد، وگرنه تا پایان
            // اعتبار کوکی می‌تواند کار کند.
            if (candidate is not null && !candidate.IsActive)
                await _signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "حساب شما به دلیل تلاش‌های ناموفق موقتاً قفل شده است. چند دقیقه دیگر تلاش کنید.");
            return View(model);
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "این حساب اجازه ورود به سیستم را ندارد.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور نادرست است.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "دسترسی مجاز نیست";
        return View();
    }
}
