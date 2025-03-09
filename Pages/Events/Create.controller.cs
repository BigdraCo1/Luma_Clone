using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

public class CreateEventModel(IConfiguration config, IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<CreateEventModel> localizer, DatabaseContext database, ISessionService sessionService, IMailService mailService) : PageModel {
    private readonly IConfiguration _config = config;
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;
    private readonly IMailService _mailService = mailService;

    [BindProperty]
    public CreateEventDto Event { get; set; } = default!;
    public IEnumerable<SelectListItem> Tags { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in?next=/events/create", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        Tags = await _database.Tag.Select(tag => new SelectListItem {
            Value = tag.Id,
            Text = CultureInfo.CurrentCulture.Name == "th" ? tag.NameTH : tag.NameEN
        }).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in?next=/events/create", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        Tags = await _database.Tag.Select(tag => new SelectListItem {
            Value = tag.Id,
            Text = CultureInfo.CurrentCulture.Name == "th" ? tag.NameTH : tag.NameEN
        }).ToListAsync();

        if (!ModelState.IsValid) {
            return Page();
        }

        var tag = _database.Tag.Find(Event.TagId);
        if (tag is null) {
            ModelState.AddModelError("Event.Tag", "Tag not found");
            return Page();
        }

        var imageData = DataUrl.Parse(Event.Image);

        var googleMapEmbedUrlMatch = Regex.Match(Event.LocationGMapUrl, @"<iframe src=""(?<url>https://www.google.com/maps/embed\?pb=[^""]+)"".*?></iframe>");
        var googleMapEmbedUrl = HtmlEncoder.Decode(googleMapEmbedUrlMatch.Groups["url"].Value);

        var evnt = new Event {
            Id = Tuid.Generate(),
            Name = Event.Name,
            Description = Event.Description,
            Image = imageData.Bytes,
            ImageType = imageData.Type,
            CreatedAt = ThDateTime.Now(),
            StartAt = Event.StartAt,
            EndAt = Event.EndAt,
            RegistrationOpen = true,
            RegistrationStartAt = Event.RegistrationStartAt,
            RegistrationEndAt = Event.RegistrationEndAt,
            Visibility = Event.Visibility,
            AutomaticApproval = Event.ApprovalType == "automatic",
            MaxParticipants = Event.MaxParticipants == "0" ? null : int.Parse(Event.MaxParticipants),
            LocationTitle = Event.LocationTitle,
            LocationSubtitle = Event.LocationSubtitle,
            LocationDescription = Event.LocationDescription,
            LocationGMapUrl = googleMapEmbedUrl,
            Tag = tag,
            Host = user,
            HostId = user.Id
        };

        if (Event.Questions is not null) {
            var i = 0;
            foreach (var question in Event.Questions) {
                await _database.Question.AddAsync(new Question {
                    Id = Tuid.Generate(),
                    Number = i,
                    Text = question,
                    Event = evnt
                });
                i++;
            }
        }

        await _database.Event.AddAsync(evnt);
        await _database.SaveChangesAsync();

        if (evnt.Visibility == Visibility.Public) {
            List<User> usersToNotify = await _database.User
                .Where(u => u.Id != user.Id && (u.FollowedTags.Any(t => t == tag) || u.Following.Any(f => f == user)))
                .ToListAsync();

            string[] addresses = [.. usersToNotify.Select(u => u.Email)];

            _ = Task.Run(() => _mailService.SendEmailAsync(addresses, _localizer["NewEvent"], MailTemplates.subscription, new Dictionary<string, string> {
                    {"newEvent", _localizer["NewEvent"]},
                    {"by", _localizer["By"]},
                    {"from", user.Name},
                    {"name", evnt.Name},
                    {"startAtDate", Formatter.FormatDate(evnt.StartAt)},
                    {"startAtTime", Formatter.FormatTime(evnt.StartAt)},
                    {"endAtTime", Formatter.FormatTime(evnt.EndAt)},
                    {"locationTitle", evnt.LocationTitle},
                    {"locationSubtitle", evnt.LocationSubtitle},
                    {"description", evnt.Description},
                    {"eventUrl", $"{_config.GetValue<string>("Public:Origin")}/events/view?id={evnt.Id}"},
                    {"viewEvent", _localizer["ViewEvent"]}
                }
            ));
        }


        return Redirect(Toast.AppendQueryString($"/events/view?id={evnt.Id}", _localizer["CreateEventSuccessful"], null, "success"));
    }
}

public class CreateEventDto() {
    [Display(Name = "EventName")]
    [Required(ErrorMessage = "RequiredError")]
    [MinLength(3, ErrorMessage = "MinLengthError")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string Name { get; set; } = default!;

    [Display(Name = "EventDescription")]
    [Required(ErrorMessage = "RequiredError")]
    [MinLength(3, ErrorMessage = "MinLengthError")]
    [MaxLength(65535, ErrorMessage = "MaxLengthError")]
    public string Description { get; set; } = default!;

    [Display(Name = "EventImage")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"^data:image/[^;]+;base64,[a-zA-Z0-9+/]+={0,2}$", ErrorMessage = "FormatError")]
    public string Image { get; set; } = default!;

    [Display(Name = "EventStartAt")]
    [Required(ErrorMessage = "RequiredError")]
    public DateTime StartAt { get; set; } = default!;

    [Display(Name = "EventEndAt")]
    [Required(ErrorMessage = "RequiredError")]
    public DateTime EndAt { get; set; } = default!;

    [Display(Name = "RegistrationStartAt")]
    [Required(ErrorMessage = "RequiredError")]
    public DateTime RegistrationStartAt { get; set; } = default!;

    [Display(Name = "RegistrationEndAt")]
    [Required(ErrorMessage = "RequiredError")]
    public DateTime RegistrationEndAt { get; set; } = default!;

    public string Visibility { get; set; } = default!;

    public string ApprovalType { get; set; } = default!;

    [Display(Name = "MaxParticipants")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"^[0-9]{1,6}$", ErrorMessage = "NumberFormatError")]
    public string MaxParticipants { get; set; } = default!;

    [Display(Name = "EventLocationTitle")]
    [Required(ErrorMessage = "RequiredError")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string LocationTitle { get; set; } = default!;

    [Display(Name = "EventLocationSubtitle")]
    [Required(ErrorMessage = "RequiredError")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string LocationSubtitle { get; set; } = default!;

    [Display(Name = "EventLocationDescription")]
    [Required(ErrorMessage = "RequiredError")]
    [MaxLength(65535, ErrorMessage = "MaxLengthError")]
    public string LocationDescription { get; set; } = default!;

    [Display(Name = "LocationGMapUrl")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"<iframe src=""https://www.google.com/maps/embed\?pb=[^""]+"".*?></iframe>", ErrorMessage = "GMapUrlFormatError")]
    public string LocationGMapUrl { get; set; } = default!;

    [Display(Name = "EventTag")]
    [Required(ErrorMessage = "RequiredError")]
    public string TagId { get; set; } = default!;

    [Display(Name = "EventRegistrationQuestions")]
    public string[]? Questions { get; set; } = default!;
}