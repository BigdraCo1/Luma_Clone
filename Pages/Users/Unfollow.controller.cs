using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

[IgnoreAntiforgeryToken(Order = 2000)]
public class UnfollowModel(IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<UnfollowModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnPostAsync(string id) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/users/profile?id={id}", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        var target = await _database.User.FindAsync(id);
        if (target is null) {
            return NotFound();
        }

        await _database.Entry(user).Collection(u => u.Following).LoadAsync();
        user.Following.Remove(target);
        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString($"/users/profile?id={id}", _localizer["UnfollowSuccess"], null, ToastTypes.Success));
    }
}