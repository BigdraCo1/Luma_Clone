using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Tags;

[IgnoreAntiforgeryToken(Order = 2000)]
public class SubscribeModel(IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<SubscribeModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnPostAsync(string id) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/tags/view?id={id}", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        var tag = await _database.Tag.FindAsync(id);
        if (tag is null) {
            return NotFound();
        }

        await _database.Entry(user).Collection(u => u.FollowedTags).LoadAsync();
        user.FollowedTags.Add(tag);
        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString($"/tags/view?id={id}", _localizer["SubscribeSuccess"], null, ToastTypes.Success));
    }
}
