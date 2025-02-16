using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;

namespace alma.Pages.Auth;

public class LogoutModel(ISessionService sessionService) : PageModel {
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAsync() {
        await _sessionService.DeleteSessionAsync(HttpContext.Request.Cookies["session"] ?? "");

        HttpContext.Response.Cookies.Delete("session");

        return Redirect("/");
    }

}
