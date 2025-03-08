using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

[IgnoreAntiforgeryToken(Order = 2000)]
public class RejectRegistrationModel(IStringLocalizer<SharedResources> sharedLocalizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnPostAsync(string eventId, string userId) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/events/view?id={eventId}", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        var evnt = await _database.Event.FindAsync(eventId);
        if (evnt is null) {
            return NotFound();
        }

        if (evnt.Host != user) {
            return Unauthorized();
        }

        var target = await _database.User.FindAsync(userId);
        if (target is null) {
            return NotFound();
        }

        var participation = await _database.UserParticipatesEvent
            .FirstOrDefaultAsync(upe => upe.UserId == target.Id && upe.EventId == evnt.Id);
        if (participation is null) {
            return NotFound();
        }
        if (participation.Status != ParticipationStatus.Pending) {
            return BadRequest();
        }

        participation.Status = ParticipationStatus.Rejected;
        await _database.SaveChangesAsync();

        return new EmptyResult();
    }
}
