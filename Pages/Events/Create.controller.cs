using System.ComponentModel.DataAnnotations;
using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using alma.Enums;
using alma.Models;
using alma.Services;

namespace alma.Pages.Events;

public class CreateEventModel(DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    [BindProperty]
    public CreateEventDto Event { get; set; } = default!;
    public IEnumerable<SelectListItem> Tags { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/events/create");
        }

        Tags = await _database.Tag.Select(tag => new SelectListItem {
            Value = tag.Id,
            Text = CultureInfo.CurrentCulture.Name == "th" ? tag.NameTH : tag.NameEN
        }).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/events/create");
        }

        if (!ModelState.IsValid) {
            return Page();
        }

        var tag = _database.Tag.Find(Event.TagId);
        if (tag is null) {
            ModelState.AddModelError("Event.Tag", "Tag not found");
            return Page();
        }

        var newEvent = new Event {
            Id = Guid.NewGuid().ToString(),
            Name = Event.Name,
            Description = Event.Description,
            Image = Convert.FromBase64String(Event.Image),
            ImageType = Event.ImageType,
            CreatedAt = DateTime.Now,
            StartAt = DateTime.Parse(Event.StartAt),
            EndAt = DateTime.Parse(Event.EndAt),
            Visibility = Event.Visibility,
            RegistrationStatus = RegistrationStatus.Pending,
            RegistrationStartAt = DateTime.Parse(Event.RegistrationStartAt),
            RegistrationEndAt = DateTime.Parse(Event.RegistrationEndAt),
            ApprovalType = Event.ApprovalType,
            MaxParticipants = Event.MaxParticipants,
            LocationTitle = Event.LocationTitle,
            LocationSubtitle = Event.LocationSubtitle,
            LocationDescription = Event.LocationDescription,
            LocationGMapUrl = Event.LocationGMapUrl,
            Tag = tag,
            Host = currentUser,
            HostId = currentUser.Id
        };

        await _database.Event.AddAsync(newEvent);
        await _database.SaveChangesAsync();

        return Redirect($"/events/{newEvent.Id}");
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

    [Display(Name = "EventImageType")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"^image/[^;]+$", ErrorMessage = "FormatError")]
    public string ImageType { get; set; } = default!;

    [Display(Name = "EventStartAt")]
    [Required(ErrorMessage = "RequiredError")]
    public string StartAt { get; set; } = default!;

    [Display(Name = "EventEndAt")]
    [Required(ErrorMessage = "RequiredError")]
    public string EndAt { get; set; } = default!;

    public string Visibility { get; set; } = default!;

    [Display(Name = "RegistrationStartAt")]
    [Required(ErrorMessage = "RequiredError")]
    public string RegistrationStartAt { get; set; } = default!;

    [Display(Name = "RegistrationEndAt")]
    [Required(ErrorMessage = "RequiredError")]
    public string RegistrationEndAt { get; set; } = default!;

    public string ApprovalType { get; set; } = default!;

    [Display(Name = "MaxParticipants")]
    [Range(1, 1000000, ErrorMessage = "RangeError")]
    public int? MaxParticipants;

    [Display(Name = "EventLocationTitle")]
    [Required(ErrorMessage = "RequiredError")]
    public string LocationTitle { get; set; } = default!;

    [Display(Name = "EventLocationSubtitle")]
    [Required(ErrorMessage = "RequiredError")]
    public string LocationSubtitle { get; set; } = default!;

    [Display(Name = "EventLocationDescription")]
    [Required(ErrorMessage = "RequiredError")]
    public string LocationDescription { get; set; } = default!;

    public string TagId { get; set; } = default!;

    [Display(Name = "LocationGMapUrl")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"<iframe src=""https://www.google.com/maps/embed\?pb=([^""]+)"".*?></iframe>", ErrorMessage = "GMapUrlFormatError")]
    public string LocationGMapUrl { get; set; } = default!;

    public string QuestionIds { get; set; } = default!;
}