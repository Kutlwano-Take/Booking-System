using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services;

namespace PracticeBookingSystem.Pages.Account;

public class LoginModel(PracticeBookingDbContext db, IPasswordHasher<AppUser> hasher) : PageModel
{
    [BindProperty] public LoginViewModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Appointments/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var login = Input.Username.Trim().ToLower();
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.IsActive && (u.Email.ToLower() == login || u.Username.ToLower() == login));

        var valid = user?.PasswordHash is not null &&
            hasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password)
                is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;

        if (!valid)
        {
            ModelState.AddModelError(string.Empty, "Incorrect email/username or password.");
            return Page();
        }

        user!.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var principal = AuthHelper.BuildPrincipal(user, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = Input.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
            });

        return !string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl)
            : RedirectToPage("/Appointments/Index");
    }
}