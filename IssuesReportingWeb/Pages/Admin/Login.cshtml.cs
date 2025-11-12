using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IssuesReportingWeb.Pages.Admin;

public class LoginModel : PageModel
{
    public class LoginInput
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    private const string DemoUser = "admin";
    private const string DemoPass = "admin123";

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input.Username)) ModelState.AddModelError("Input.Username", "Username is required");
        if (string.IsNullOrWhiteSpace(Input.Password)) ModelState.AddModelError("Input.Password", "Password is required");
        if (!ModelState.IsValid) return Page();

        if (Input.Username != DemoUser || Input.Password != DemoPass)
        {
            ErrorMessage = "Invalid credentials";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, DemoUser),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = false });

        return RedirectToPage("Index");
    }
}