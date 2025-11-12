// Build full ASP.NET Razor Page only when not targeting Windows desktop TFM.
#if !WINDOWS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IssuesReportingWeb.Pages.Admin;

[Authorize(Roles = "Admin")]
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPost()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Admin/Login");
    }
}
#else
namespace IssuesReportingWeb.Pages.Admin
{
    // Safe stub for desktop build to avoid ASP.NET references
    public class LogoutModel { }
}
#endif