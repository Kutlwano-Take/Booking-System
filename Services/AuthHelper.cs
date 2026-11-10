using System.Security.Claims;
using PracticeBookingSystem.Models;

namespace PracticeBookingSystem.Services;

public static class AuthHelper
{
    public const string JobTitleClaim = "job_title";

    public static ClaimsPrincipal BuildPrincipal(AppUser user, string scheme)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(JobTitleClaim, user.JobTitle)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, scheme));
    }

    public static string DisplayName(this ClaimsPrincipal p) =>
        p.FindFirst(ClaimTypes.Name)?.Value ?? "Guest";

    public static string JobTitle(this ClaimsPrincipal p) =>
        p.FindFirst(JobTitleClaim)?.Value ?? "";

    public static string Initials(this ClaimsPrincipal p)
    {
        var parts = p.DisplayName().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpper(),
            _ => (parts[0][..1] + parts[^1][..1]).ToUpper()
        };
    }
}