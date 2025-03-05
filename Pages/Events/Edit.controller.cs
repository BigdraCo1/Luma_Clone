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

public class EditEventModel(IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<EditEventModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public Event ExistingEvent { get; set; } = default!;
    public IEnumerable<SelectListItem> Tags { get; set; } = default!;

    [BindProperty]
    public EditEventDto UpdatedEvent { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/events/edit?id={id}", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        var existingEvent = await _database.Event
            .Include(e => e.Questions)
            .Include(e => e.Participants)
            .Include(e => e.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent is null) {
            return NotFound();
        }

        Tags = await _database.Tag.Select(tag => new SelectListItem {
            Value = tag.Id,
            Text = CultureInfo.CurrentCulture.Name == "th" ? tag.NameTH : tag.NameEN
        }).ToListAsync();

        ExistingEvent = existingEvent;

        var locationGMapUrl = $"<iframe src=\"{existingEvent.LocationGMapUrl}\" width=\"600\" height=\"450\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>";

        UpdatedEvent = new EditEventDto {
            Id = existingEvent.Id,
            Name = existingEvent.Name,
            Description = existingEvent.Description,
            Image = DataUrl.Create(existingEvent.Image, existingEvent.ImageType),
            StartAt = existingEvent.StartAt,
            EndAt = existingEvent.EndAt,
            RegistrationOpen = existingEvent.RegistrationOpen ? "true" : "false",
            RegistrationStartAt = existingEvent.RegistrationStartAt,
            RegistrationEndAt = existingEvent.RegistrationEndAt,
            Visibility = existingEvent.Visibility,
            ApprovalType = existingEvent.ApprovalType,
            MaxParticipants = existingEvent.MaxParticipants?.ToString() ?? "0",
            LocationTitle = existingEvent.LocationTitle,
            LocationSubtitle = existingEvent.LocationSubtitle,
            LocationDescription = existingEvent.LocationDescription,
            LocationGMapUrl = locationGMapUrl,
            TagId = existingEvent.Tag.Id
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect(Toast.AppendQueryString($"/auth/sign-in?next=/events/create", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        Tags = await _database.Tag.Select(tag => new SelectListItem {
            Value = tag.Id,
            Text = CultureInfo.CurrentCulture.Name == "th" ? tag.NameTH : tag.NameEN
        }).ToListAsync();

        if (!ModelState.IsValid) {
            return Page();
        }

        var tag = _database.Tag.Find(UpdatedEvent.TagId);
        if (tag is null) {
            ModelState.AddModelError("Event.Tag", "Tag not found");
            return Page();
        }

        var existingEvent = await _database.Event.FindAsync(UpdatedEvent.Id);
        if (existingEvent is null) {
            return NotFound();
        }

        var imageData = DataUrl.Parse(UpdatedEvent.Image);

        var googleMapEmbedUrlMatch = Regex.Match(UpdatedEvent.LocationGMapUrl, @"<iframe src=""(?<url>https://www.google.com/maps/embed\?pb=[^""]+)"".*?></iframe>");
        var googleMapEmbedUrl = googleMapEmbedUrlMatch.Groups["url"].Value;

        existingEvent.Name = UpdatedEvent.Name;
        existingEvent.Description = UpdatedEvent.Description;
        existingEvent.Image = imageData.Bytes;
        existingEvent.ImageType = imageData.Type;
        existingEvent.StartAt = UpdatedEvent.StartAt;
        existingEvent.EndAt = UpdatedEvent.EndAt;
        existingEvent.RegistrationOpen = UpdatedEvent.RegistrationOpen == "true";
        existingEvent.RegistrationStartAt = UpdatedEvent.RegistrationStartAt;
        existingEvent.RegistrationEndAt = UpdatedEvent.RegistrationEndAt;
        existingEvent.Visibility = UpdatedEvent.Visibility;
        existingEvent.ApprovalType = UpdatedEvent.ApprovalType;
        existingEvent.MaxParticipants = UpdatedEvent.MaxParticipants == "0" ? null : int.Parse(UpdatedEvent.MaxParticipants);
        existingEvent.LocationTitle = UpdatedEvent.LocationTitle;
        existingEvent.LocationSubtitle = UpdatedEvent.LocationSubtitle;
        existingEvent.LocationDescription = UpdatedEvent.LocationDescription;
        existingEvent.LocationGMapUrl = googleMapEmbedUrl;
        existingEvent.Tag = tag;

        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString($"/events/edit?id={existingEvent.Id}", _localizer["EventUpdateSuccessful"], null, "success"));
    }
}

public class EditEventDto() {
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Image { get; set; } = default!;
    public DateTime StartAt { get; set; } = default!;
    public DateTime EndAt { get; set; } = default!;
    public string RegistrationOpen { get; set; } = default!;
    public DateTime RegistrationStartAt { get; set; } = default!;
    public DateTime RegistrationEndAt { get; set; } = default!;
    public string Visibility { get; set; } = default!;
    public string ApprovalType { get; set; } = default!;
    public string MaxParticipants { get; set; } = default!;
    public string LocationTitle { get; set; } = default!;
    public string LocationSubtitle { get; set; } = default!;
    public string LocationDescription { get; set; } = default!;
    public string LocationGMapUrl { get; set; } = default!;
    public string TagId { get; set; } = default!;
}
