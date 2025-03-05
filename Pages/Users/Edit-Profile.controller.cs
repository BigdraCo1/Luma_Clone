using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

public class EditProfileModel(IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<EditProfileModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public User ExistingUser { get; set; } = default!;

    [BindProperty]
    public UpdateUserDto UpdatedUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var existingUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (existingUser is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in?next=/users/edit-profile", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        ExistingUser = existingUser;

        UpdatedUser = new UpdateUserDto {
            Name = existingUser.Name,
            Username = existingUser.Username,
            Bio = existingUser.Bio,
            InstagramUsername = existingUser.InstagramUsername,
            TwitterUsername = existingUser.TwitterUsername,
            YoutubeUsername = existingUser.YoutubeUsername,
            TikTokUsername = existingUser.TikTokUsername,
            LinkedinHandle = existingUser.LinkedinHandle,
            WebsiteUrl = existingUser.WebsiteUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        var existingUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (existingUser is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in?next=/users/edit-profile", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        if (!ModelState.IsValid) {
            ExistingUser = existingUser;
            return Page();
        }

        if (existingUser.Username != UpdatedUser.Username) {
            var existingUsersWithUsername = _database.User.Where(user => user.Username == UpdatedUser.Username);
            if (existingUsersWithUsername.Any()) {
                ModelState.AddModelError("UpdatedUser.Username", _localizer["UsernameTakenError"]);
                ExistingUser = existingUser;
                return Page();
            }
        }

        existingUser.Name = UpdatedUser.Name;
        existingUser.Username = UpdatedUser.Username;
        existingUser.Bio = UpdatedUser.Bio;
        existingUser.InstagramUsername = UpdatedUser.InstagramUsername;
        existingUser.TwitterUsername = UpdatedUser.TwitterUsername;
        existingUser.YoutubeUsername = UpdatedUser.YoutubeUsername;
        existingUser.TikTokUsername = UpdatedUser.TikTokUsername;
        existingUser.LinkedinHandle = UpdatedUser.LinkedinHandle;
        existingUser.WebsiteUrl = UpdatedUser.WebsiteUrl;

        await _database.SaveChangesAsync();

        return Redirect(Toast.AppendQueryString("/users/edit-profile", _localizer["ProfileUpdateSuccessful"], null, "success"));
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
