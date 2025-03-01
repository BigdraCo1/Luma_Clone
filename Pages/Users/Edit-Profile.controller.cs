using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

public class EditProfileModel(IStringLocalizer<EditProfileModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    [BindProperty]
    public UpdateUserDto UpdatedUser { get; set; } = default!;
    public User CurrentUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/users/edit-profile");
        }
        UpdatedUser = new UpdateUserDto {
            Name = currentUser.Name,
            Username = currentUser.Username,
            Bio = currentUser.Bio,
            InstagramUsername = currentUser.InstagramUsername,
            TwitterUsername = currentUser.TwitterUsername,
            YoutubeUsername = currentUser.YoutubeUsername,
            TikTokUsername = currentUser.TikTokUsername,
            LinkedinHandle = currentUser.LinkedinHandle,
            WebsiteUrl = currentUser.WebsiteUrl
        };
        CurrentUser = currentUser;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/users/edit-profile");
        }

        if (!ModelState.IsValid) {
            CurrentUser = currentUser;
            return Page();
        }

        if (currentUser.Username != UpdatedUser.Username) {
            var existingUsersWithUsername = _database.User.Where(user => user.Username == UpdatedUser.Username);
            if (existingUsersWithUsername.Any()) {
                ModelState.AddModelError("UpdatedUser.Username", _localizer["UsernameTakenError"]);
                CurrentUser = currentUser;
                return Page();
            }
        }

        currentUser.Name = UpdatedUser.Name;
        currentUser.Username = UpdatedUser.Username;
        currentUser.Bio = UpdatedUser.Bio;
        currentUser.InstagramUsername = UpdatedUser.InstagramUsername;
        currentUser.TwitterUsername = UpdatedUser.TwitterUsername;
        currentUser.YoutubeUsername = UpdatedUser.YoutubeUsername;
        currentUser.TikTokUsername = UpdatedUser.TikTokUsername;
        currentUser.LinkedinHandle = UpdatedUser.LinkedinHandle;
        currentUser.WebsiteUrl = UpdatedUser.WebsiteUrl;

        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString("/Users/Edit-Profile", _localizer["ProfileUpdateSuccessful"], null, "success"));
    }
}

public class UpdateUserDto() {
    [Display(Name = "DisplayName")]
    [Required(ErrorMessage = "RequiredError")]
    [MinLength(3, ErrorMessage = "MinLengthError")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string Name { get; set; } = default!;

    [Display(Name = "Username")]
    [Required(ErrorMessage = "RequiredError")]
    [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "AlphanumericFormatError")]
    [MinLength(3, ErrorMessage = "MinLengthError")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string Username { get; set; } = default!;

    [Display(Name = "Bio")]
    [MaxLength(65535, ErrorMessage = "MaxLengthError")]
    public string? Bio { get; set; }

    [Display(Name = "InstagramUsername")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? InstagramUsername { get; set; }

    [Display(Name = "TwitterUsername")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? TwitterUsername { get; set; }

    [Display(Name = "YoutubeUsername")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? YoutubeUsername { get; set; }

    [Display(Name = "TikTokUsername")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? TikTokUsername { get; set; }

    [Display(Name = "LinkedinHandle")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? LinkedinHandle { get; set; }

    [Display(Name = "WebsiteUrl")]
    [MaxLength(255, ErrorMessage = "MaxLengthError")]
    public string? WebsiteUrl { get; set; }
}
