using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

public class RegisteredEventsModel(IStringLocalizer<SharedResources> sharedLocalizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public List<Event> Events { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/events/my-events", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        await _database.Entry(user).Collection(u => u.ParticipatingEvents).LoadAsync();
        Events = [.. user.ParticipatingEvents.Where(e => e.EndAt > ThDateTime.Now() && e.Visibility != Visibility.Private).OrderBy(e => e.StartAt)];

        return Page();
    }
}
