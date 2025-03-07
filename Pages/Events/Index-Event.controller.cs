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
        private readonly EmailService _emailService;
        private readonly IStringLocalizer _localizer;

        public IndexEventModel(DatabaseContext database, ISessionService sessionService, EmailService emailService, IStringLocalizer<IndexEventModel> localizer, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _database = database;
            _sessionService = sessionService;
            _sharedLocalizer = sharedLocalizer;
            _emailService = emailService;
            _localizer = localizer;
        }

        public Event currentEvent { get; set; }
        public IList<User> GoingAttendees { get; set; } = new List<User>();
        public IList<User> DisplayAttendees { get; set; } = new List<User>();
        public string IFramSrc { get; set; }
        public User currentUser { get; set; }
        public bool registered { get; set; }
        public string Status { get; set; }
        public bool full { get; set; } = false;

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

            Status = (await _database.UserAttendEvent
                .FirstOrDefaultAsync(uae => uae.UserId == currentUser.Id && uae.EventId == tuid))?.Status;

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

            if (currentEvent.Participants.Count >= currentEvent.MaxParticipants)
            {
                full = true;
                return StatusCode(500);
            }

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

            var icsContent = _emailService.GenerateIcsContent(
                    currentEvent.Name,
                    currentEvent.Description,
                    currentEvent.StartAt,
                    currentEvent.EndAt,
                    currentEvent.LocationDescription,
                    currentEvent.Host.Email,
                    currentUser.Email,
                    "ACCEPTED"
                    );

            Status = (await _database.UserAttendEvent
                .FirstOrDefaultAsync(uae => uae.UserId == currentUser.Id && uae.EventId == currentEvent.Id))?.Status;

            var htmlContent = string.Empty;

            if (Status == ParticipantStatus.Going)
            {
                htmlContent = $@"
                <html>
                <head>
                    <meta charset='UTF-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                    <title>Event - Invitation</title>
                    <style>
                        body {{
                            margin: 0;
                            padding: 0;
                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                        }}
                        .container {{
                            width: 100%;
                            padding: 20px;
                        }}
                        .content {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        }}
                        .header {{
                            padding: 20px;
                            background-color: #000480;
                            color: white;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 20px;
                            font-weight: 500;
                            color: #329e00;
                        }}
                        .header h2 {{
                            margin: 10px 0 0;
                            font-size: 28px;
                            font-weight: 700;
                            color: white;
                        }}
                        .section {{
                            padding: 20px;
                            border-bottom: 1px solid #eee;
                        }}
                        .section .detail {{
                            margin: 0;
                            color: #666;
                            font-size: 16px;
                            display: flex;
                            align-items: center;
                            flex-direction: row;
                            gap: 10px;
                        }}
                        .section .detail strong {{
                            color: #333;
                        }}
                        .section .detail img {{
                            margin-right: 10px; /* Add space between image and text */
                            vertical-align: middle; /* Center image vertically */
                        }}
                        .footer {{
                            padding: 10px 30px;
                            text-align: center;
                            border-top: 1px solid #eee;
                        }}
                        .footer p {{
                            margin: 0;
                            color: #666;
                            font-size: 14px;
                        }}
                        .cta {{
                            padding: 0 20px 30px;
                            text-align: center;
                        }}
                        .cta a {{
                            display: inline-block;
                            padding: 14px 30px;
                            text-decoration: none;
                            font-weight: 500;
                            border-radius: 6px;
                            font-size: 16px;
                        }}
                        .cta .primary {{
                            background-color: #000480;
                            color: white;
                        }}
                        .cta .secondary {{
                            color: #000480;
                            border: 1px solid #000480;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td align='center'>
                                    <table role='presentation' class='content'>
                                        <tr>
                                            <td class='header'>
                                                <img src='https://ipfs.io/ipfs/bafkreighva3y45p2oc2pw743u6e47haohnsx7yrcssiw4ftd3g7eilwbyq' alt='Green check mark' width='25' height='25'>
                                                <h1>
                                                    You have registered for
                                                </h1>
                                                <h2>{currentEvent.Name}</h2>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='section'>
                                                <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreigg3n5h7q337f5igbevo2qp6jf6yyihpqdvdmcpqrgwlkkukhnyyu' alt='calendar icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong>{currentEvent.StartAt:ddd, MMMM dd}</strong>
                                                                    <br />
                                                                    <span>{currentEvent.StartAt:hh:mm tt} - {currentEvent.EndAt:hh:mm tt} GMT+7</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreid24bbifw2yg4piw6javuvxxd33mhmmhjkq2y6bul2rbaqk5mbxq4' alt='location icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong><a href='#'>{currentEvent.LocationTitle}</a></strong>
                                                                    <br />
                                                                    <span>{currentEvent.LocationSubtitle}</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <!-- Message -->
                                        <tr>
                                            <td class='section'>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    {currentEvent.Description}
                                                </p>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    {_localizer["ThankYouForJoiningEvent"]}
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='cta'>
                                                <a href='#' class='primary'>{_localizer["EventPage"]}</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='footer'>
                                                <p>&copy; 2025 Alma. All Rights Reserved.</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(currentUser.Email, currentEvent.Name, htmlContent, icsContent);
            }
            else
            {
                htmlContent = $@"
                <html>
                <head>
                    <meta charset='UTF-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                    <title>Event - Invitation</title>
                    <style>
                        body {{
                            margin: 0;
                            padding: 0;
                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                        }}
                        .container {{
                            width: 100%;
                            padding: 20px;
                        }}
                        .content {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        }}
                        .header {{
                            padding: 20px;
                            background-color: #000480;
                            color: white;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 20px;
                            font-weight: 500;
                            color: #329e00;
                        }}
                        .header h2 {{
                            margin: 10px 0 0;
                            font-size: 28px;
                            font-weight: 700;
                            color: white;
                        }}
                        .section {{
                            padding: 20px;
                            border-bottom: 1px solid #eee;
                        }}
                        .section .detail {{
                            margin: 0;
                            color: #666;
                            font-size: 16px;
                            display: flex;
                            align-items: center;
                            flex-direction: row;
                            gap: 10px;
                        }}
                        .section .detail strong {{
                            color: #333;
                        }}
                        .section .detail img {{
                            margin-right: 10px; /* Add space between image and text */
                            vertical-align: middle; /* Center image vertically */
                        }}
                        .footer {{
                            padding: 10px 30px;
                            text-align: center;
                            border-top: 1px solid #eee;
                        }}
                        .footer p {{
                            margin: 0;
                            color: #666;
                            font-size: 14px;
                        }}
                        .cta {{
                            padding: 0 20px 30px;
                            text-align: center;
                        }}
                        .cta a {{
                            display: inline-block;
                            padding: 14px 30px;
                            text-decoration: none;
                            font-weight: 500;
                            border-radius: 6px;
                            font-size: 16px;
                        }}
                        .cta .primary {{
                            background-color: #000480;
                            color: white;
                        }}
                        .cta .secondary {{
                            color: #000480;
                            border: 1px solid #000480;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td align='center'>
                                    <table role='presentation' class='content'>
                                        <tr>
                                            <td class='header'>
                                                <img src='https://ipfs.io/ipfs/bafkreighva3y45p2oc2pw743u6e47haohnsx7yrcssiw4ftd3g7eilwbyq' alt='Green check mark' width='25' height='25'>
                                                <h1>
                                                    You have registered for
                                                </h1>
                                                <h2>{currentEvent.Name}</h2>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='section'>
                                                <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreigg3n5h7q337f5igbevo2qp6jf6yyihpqdvdmcpqrgwlkkukhnyyu' alt='calendar icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong>{currentEvent.StartAt:ddd, MMMM dd}</strong>
                                                                    <br />
                                                                    <span>{currentEvent.StartAt:hh:mm tt} - {currentEvent.EndAt:hh:mm tt} GMT+7</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreid24bbifw2yg4piw6javuvxxd33mhmmhjkq2y6bul2rbaqk5mbxq4' alt='location icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong><a href='#'>{currentEvent.LocationTitle}</a></strong>
                                                                    <br />
                                                                    <span>{currentEvent.LocationSubtitle}</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <!-- Message -->
                                        <tr>
                                            <td class='section'>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    {currentEvent.Description}
                                                </p>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    {_localizer["WaitingForApproval"]}
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='cta'>
                                                <a href='#' class='primary'>{_localizer["EventPage"]}</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='footer'>
                                                <p>&copy; 2025 Alma. All Rights Reserved.</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(currentUser.Email, currentEvent.Name, htmlContent);
            }

            return RedirectToPage("/Events/Index-Event", new { tuid = Register.EventId });
        }
    }
}

public class RegisterDto
{
    public string EventId { get; set; }
    public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
}