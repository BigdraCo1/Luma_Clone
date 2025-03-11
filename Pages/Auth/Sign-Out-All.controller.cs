using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Auth;

public class SignOutAllModel(IStringLocalizer<SignOutAllModel> localizer, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAsync() {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");

        if (user is null) {
            return Redirect(Toast.AppendQueryString("/", _localizer["NotLoggedIn"], _localizer["NotLoggedInDescription"], ToastTypes.Error));
        }

        await _sessionService.DeleteSessionsForUserAsync(user);

        HttpContext.Response.Cookies.Delete("session");

        return Redirect(Toast.AppendQueryString("/", _localizer["LoggedOut"], null, ToastTypes.Success));
    }

}
