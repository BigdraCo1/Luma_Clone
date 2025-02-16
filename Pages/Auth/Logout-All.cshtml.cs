using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;

namespace alma.Pages.Auth;

public class LogoutAllModel(ISessionService sessionService) : PageModel {
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAsync() {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");

        if (user is null) {
            return Redirect("/");
        }

        await _sessionService.DeleteSessionsForUserAsync(user);

        HttpContext.Response.Cookies.Delete("session");

        return Redirect("/");
    }

}
