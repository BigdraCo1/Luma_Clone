using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

[IgnoreAntiforgeryToken(Order = 2000)]
public class AcceptRegistrationModel(IConfiguration config, IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<AcceptRegistrationModel> localizer, DatabaseContext database, ISessionService sessionService, IMailService mailService) : PageModel {
    private readonly IConfiguration _config = config;
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;
    private readonly IMailService _mailService = mailService;

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

        participation.Status = ParticipationStatus.Accepted;
        await _database.SaveChangesAsync();

        _ = Task.Run(() => _mailService.SendEmailAsync([user.Email], _localizer["RegistrationAccepted"], MailTemplates.accepted, new Dictionary<string, string> {
                { "accepted", _localizer["YouHaveBeenAccepted"] },
                { "name", evnt.Name },
                { "startAtDate", Formatter.FormatDate(evnt.StartAt) },
                { "startAtTime", Formatter.FormatTime(evnt.StartAt) },
                { "endAtTime", Formatter.FormatTime(evnt.EndAt) },
                { "locationTitle", evnt.LocationTitle },
                { "locationSubtitle", evnt.LocationSubtitle },
                { "welcomeMessage", _localizer["WelcomeMessage"] },
                { "eventUrl", $"{_config.GetValue<string>("Public:Origin")}/events/view?id={evnt.Id}" },
                { "eventPage", _localizer["EventPage"] }
            },
            icsContent: Ics.GenerateIcsString(evnt.Name, evnt.Description, evnt.StartAt, evnt.EndAt, evnt.LocationTitle, user.Email, target.Email)
            ));

        return new EmptyResult();
    }
}
