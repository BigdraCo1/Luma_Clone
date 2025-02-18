using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Services;
using alma.Utils;

namespace alma.Pages.Auth;

public class LogoutModel(IStringLocalizer<LogoutModel> localizer, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAsync() {
        await _sessionService.DeleteSessionAsync(HttpContext.Request.Cookies["session"] ?? "");

        HttpContext.Response.Cookies.Delete("session");

        return Redirect(Toast.AppendQueryString("/", _localizer["LoggedOut"], null, "success"));
    }
}
