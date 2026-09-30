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

        // Ø­Ø³Ø§Ø¨ ØºÛŒØ±ÙØ¹Ø§Ù„ Ø¨Ø§ÛŒØ¯ Ù†ØªÙˆØ§Ù†Ø¯ ÙˆØ§Ø±Ø¯ Ø´ÙˆØ¯. Ø§ÛŒÙ† Ø¨Ø±Ø±Ø³ÛŒ Ù‚Ø¨Ù„ Ø§Ø² PasswordSignInAsync
        // Ø§Ù†Ø¬Ø§Ù… Ù…ÛŒâ€ŒØ´ÙˆØ¯ ØªØ§ Ù†ØªÛŒØ¬Ù‡ Ø¨Ù‡ Ú©Ø§Ø±Ø¨Ø± Ù„Ùˆ Ù†Ø±ÙˆØ¯ Ú©Ù‡ Ø±Ù…Ø² Ø¯Ø±Ø³Øª Ø§Ø³Øª ÛŒØ§ Ù†Ù‡ØŒ Ùˆ Ø¯Ø±
        // Ù†ØªÛŒØ¬Ù‡ PasswordSignInAsync Ù‡Ù… Ø¯ÙˆØ¨Ø§Ø±Ù‡ Ø§Ù†Ø¬Ø§Ù… Ù†Ù…ÛŒâ€ŒØ´ÙˆØ¯.
        var candidate = await _userManager.FindByNameAsync(model.UserName);
        if (candidate is { IsActive: false })
        {
            ModelState.AddModelError(string.Empty, "Ø§ÛŒÙ† Ø­Ø³Ø§Ø¨ ØºÛŒØ±ÙØ¹Ø§Ù„ Ø´Ø¯Ù‡ Ø§Ø³Øª. Ø¨Ø§ Ù…Ø¯ÛŒØ± Ø³Ø§Ù…Ø§Ù†Ù‡ ØªÙ…Ø§Ø³ Ø¨Ú¯ÛŒØ±ÛŒØ¯.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // ØºÛŒØ±ÙØ¹Ø§Ù„ Ú©Ø±Ø¯Ù† Ú©Ø§Ø±Ø¨Ø± Ø¨Ø§ÛŒØ¯ Ù†Ø´Ø³Øª Ø¨Ø§Ø² Ø§Ùˆ Ø±Ø§ Ù‡Ù… Ø¨Ø¨Ù†Ø¯Ø¯ØŒ ÙˆÚ¯Ø±Ù†Ù‡ ØªØ§ Ù¾Ø§ÛŒØ§Ù†
            // Ø§Ø¹ØªØ¨Ø§Ø± Ú©ÙˆÚ©ÛŒ Ù…ÛŒâ€ŒØªÙˆØ§Ù†Ø¯ Ú©Ø§Ø± Ú©Ù†Ø¯.
            if (candidate is not null && !candidate.IsActive)
                await _signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Ø­Ø³Ø§Ø¨ Ø´Ù…Ø§ Ø¨Ù‡ Ø¯Ù„ÛŒÙ„ ØªÙ„Ø§Ø´â€ŒÙ‡Ø§ÛŒ Ù†Ø§Ù…ÙˆÙÙ‚ Ù…ÙˆÙ‚ØªØ§Ù‹ Ù‚ÙÙ„ Ø´Ø¯Ù‡ Ø§Ø³Øª. Ú†Ù†Ø¯ Ø¯Ù‚ÛŒÙ‚Ù‡ Ø¯ÛŒÚ¯Ø± ØªÙ„Ø§Ø´ Ú©Ù†ÛŒØ¯.");
            return View(model);
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Ø§ÛŒÙ† Ø­Ø³Ø§Ø¨ Ø§Ø¬Ø§Ø²Ù‡ ÙˆØ±ÙˆØ¯ Ø¨Ù‡ Ø³ÛŒØ³ØªÙ… Ø±Ø§ Ù†Ø¯Ø§Ø±Ø¯.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Ù†Ø§Ù… Ú©Ø§Ø±Ø¨Ø±ÛŒ ÛŒØ§ Ø±Ù…Ø² Ø¹Ø¨ÙˆØ± Ù†Ø§Ø¯Ø±Ø³Øª Ø§Ø³Øª.");
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
        ViewData["Title"] = "Ø¯Ø³ØªØ±Ø³ÛŒ Ù…Ø¬Ø§Ø² Ù†ÛŒØ³Øª";
        return View();
    }
}
