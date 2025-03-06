using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
using alma.Enums;
using alma.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace alma.Pages.Events
{
    public class IndexEventModel : PageModel
    {
        private readonly DatabaseContext _database;
        private readonly ISessionService _sessionService;
        private readonly IStringLocalizer _sharedLocalizer;

        public IndexEventModel(DatabaseContext database, ISessionService sessionService, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _database = database;
            _sessionService = sessionService;
            _sharedLocalizer = sharedLocalizer;
        }

        public Event currentEvent { get; set; }
        public IList<User> GoingAttendees { get; set; } = new List<User>();
        public IList<User> DisplayAttendees { get; set; } = new List<User>();
        public string IFramSrc { get; set; }
        public User currentUser { get; set; }
        public bool registered { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {

            var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
            if (user is null)
            {
                currentUser = null;
            }
            else
            {
                currentUser = user;
            }

            currentEvent = await _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Include(e => e.Participants)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(i => i.Id == tuid);

            if (currentEvent == null)
            {
                return NotFound();
            }
            
            registered = currentEvent.Participants.Any(p => p.Id == currentUser.Id);

            GoingAttendees = await _database.UserAttendEvent
                .Where(uae => uae.EventId == tuid && uae.Status == ParticipantStatus.Going)
                .Join(
                    _database.User,
                    uae => uae.UserId,
                    user => user.Id,
                    (uae, user) => user
                )
.ToListAsync();

            DisplayAttendees = await _database.UserAttendEvent
    .Where(uae => uae.EventId == tuid && uae.Status == ParticipantStatus.Going)
    .Join(
        _database.User,
                uae => uae.UserId,
        user => user.Id,
                    (uae, user) => user
    )
    .Take(6)
    .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetImageAsync(string tuid)
        {
            if (string.IsNullOrEmpty(tuid))
            {
                return BadRequest("Invalid event ID.");
            }

            try
            {
                var eventItem = await _database.Event
                    .FirstOrDefaultAsync(e => e.Id == tuid);

                if (eventItem == null || eventItem.Image == null || eventItem.ImageType == null)
                {
                    return NotFound("Event or image not found.");
                }

                return File(eventItem.Image, eventItem.ImageType);
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        [BindProperty]
        public RegisterDto Register { get; set; } = new RegisterDto();

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
            if (currentUser is null)
            {
                return Redirect(Toast.AppendQueryString("/auth/sign-in?next=/events/create", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            currentEvent = await _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Include(e => e.Participants)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(i => i.Id == Register.EventId);

            var participantStatus = ParticipantStatus.Pending;

            if (currentEvent.ApprovalType == "automatic")
            {
                participantStatus = ParticipantStatus.Going;
            }

            var userAttendEvent = new UserAttendEvent
            {
                UserId = currentUser.Id,
                EventId = Register.EventId,
                Status = participantStatus
            };

            await _database.UserAttendEvent.AddAsync(userAttendEvent);

            currentEvent.Participants.Add(currentUser);

            foreach (var question in currentEvent.Questions)
            {
                if (Register.Answers.TryGetValue(question.Id, out var answerText))
                {
                    var newAnswer = new Answer
                    {
                        Id = Tuid.Generate(),
                        Text = answerText,
                        Question = question,
                        User = currentUser
                    };

                    await _database.Answer.AddAsync(newAnswer);
                }
            }

            await _database.SaveChangesAsync();

            return RedirectToPage("/Events/Index-Event", new { tuid = Register.EventId });
        }
    }
}

public class RegisterDto
{
    public string EventId { get; set; }
    public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
}