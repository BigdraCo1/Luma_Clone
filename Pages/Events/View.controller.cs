using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

public class ViewEventModel(IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<ViewEventModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public new User? User { get; set; }
    public Event Event { get; set; } = default!;
    public ICollection<User> AcceptedParticipants { get; set; } = default!;
    public string? RegistrationStatus { get; set; } = default!;
    public List<Registration> PendingRegistrations { get; set; } = default!;

    [BindProperty]
    public RegisterEventDto RegistrationData { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        User = user;

        var evnt = await _database.Event
            .Include(e => e.Host)
            .Include(e => e.Tag)
            .Include(e => e.Participants)
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evnt is null) {
            return NotFound();
        }

        if (evnt.Visibility == Visibility.Private && user != evnt.Host) {
            return NotFound();
        }

        Event = evnt;

        AcceptedParticipants = [.. evnt.Participants
            .Where(p => _database.UserParticipatesEvent
                .Any(upe => upe.UserId == p.Id && upe.EventId == evnt.Id && upe.Status == ParticipationStatus.Accepted))];

        RegistrationStatus = user is not null ? _database.UserParticipatesEvent
            .FirstOrDefault(upe => upe.UserId == user.Id && upe.EventId == evnt.Id)?.Status : null;

        var pendingParticipations = await _database.UserParticipatesEvent
            .Where(upe => upe.EventId == evnt.Id && upe.Status == ParticipationStatus.Pending)
            .ToListAsync();

        PendingRegistrations = [];

        foreach (var pendingParticipation in pendingParticipations) {
            var pendingUser = await _database.User.FindAsync(pendingParticipation.UserId);
            if (pendingUser is null) {
                continue;
            }
            List<QuestionAnswer> questionAnswers = [];
            foreach (var question in evnt.Questions.OrderBy(q => q.Number)) {
                var answer = await _database.Answer
                    .FirstOrDefaultAsync(a => a.User == pendingUser && a.Question == question);
                questionAnswers.Add(new QuestionAnswer {
                    Question = question,
                    Answer = answer!
                });
            }
            PendingRegistrations.Add(new Registration {
                User = pendingUser,
                QuestionAnswers = questionAnswers
            });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/events/view?id={id}", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        var evnt = await _database.Event.FindAsync(id);
        if (evnt is null) {
            return NotFound();
        }

        await _database.UserParticipatesEvent.AddAsync(new UserParticipatesEvent {
            UserId = user.Id,
            EventId = evnt.Id,
            Status = evnt.AutomaticApproval ? ParticipationStatus.Accepted : ParticipationStatus.Pending
        });

        if (RegistrationData.Answers is not null) {
            foreach (var (questionId, answer) in RegistrationData.Answers) {
                var question = await _database.Question.FindAsync(questionId);
                if (question is null) {
                    continue;
                }
                await _database.Answer.AddAsync(new Answer {
                    Id = Tuid.Generate(),
                    Text = answer,
                    Question = question,
                    User = user
                });
            }
        }

        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString($"/events/view?id={id}", _localizer["RegistrationSuccessful"], null, ToastTypes.Success));
    }
}

public class RegisterEventDto {
    public Dictionary<string, string>? Answers { get; set; } = default!;
}

public class Registration {
    public User User { get; set; } = default!;
    public List<QuestionAnswer> QuestionAnswers { get; set; } = default!;
}

public class QuestionAnswer {
    public Question Question { get; set; } = default!;
    public Answer Answer { get; set; } = default!;
}
