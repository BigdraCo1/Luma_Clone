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
    public UpdateUserModel UpdatedUser { get; set; } = default!;
    public User CurrentUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/users/edit-profile");
        }
        UpdatedUser = new UpdateUserModel();
        UpdatedUser.Name = currentUser.Name;
        UpdatedUser.Username = currentUser.Username;
        UpdatedUser.Bio = currentUser.Bio;
        UpdatedUser.InstagramUsername = currentUser.InstagramUsername;
        UpdatedUser.TwitterUsername = currentUser.TwitterUsername;
        UpdatedUser.YoutubeUsername = currentUser.YoutubeUsername;
        UpdatedUser.TikTokUsername = currentUser.TikTokUsername;
        UpdatedUser.LinkedinHandle = currentUser.LinkedinHandle;
        UpdatedUser.WebsiteUrl = currentUser.WebsiteUrl;
        CurrentUser = currentUser;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/sign-in?next=/users/edit-profile");
        }

        if (!ModelState.IsValid) {
            foreach (var modelState in ModelState.Values) {
                foreach (var error in modelState.Errors) {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            CurrentUser = currentUser;
            return Page();
        }

        currentUser.Name = UpdatedUser.Name;
        currentUser.Username = UpdatedUser.Username;
        currentUser.Bio = UpdatedUser.Bio ?? "";
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

public class UpdateUserModel() {
    [Display(Name = "display name")]
    [MinLength(3, ErrorMessage = "DisplayNameMinLength")]
    [MaxLength(255)]
    public string Name { get; set; } = default!;

    [Display(Name = "username")]
    [RegularExpression(@"^[a-zA-Z0-9\-_]+$")]
    [MinLength(3)]
    [MaxLength(255)]
    public string Username { get; set; } = default!;

    public string? Bio { get; set; }
    public string? InstagramUsername { get; set; }
    public string? TwitterUsername { get; set; }
    public string? YoutubeUsername { get; set; }
    public string? TikTokUsername { get; set; }
    public string? LinkedinHandle { get; set; }
    public string? WebsiteUrl { get; set; }
}