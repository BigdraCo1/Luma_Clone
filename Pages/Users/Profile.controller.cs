using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

public class ProfileModel(IStringLocalizer<SharedResources> sharedLocalizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _sharedLocalizer = sharedLocalizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public User CurrentUser { get; set; } = default!;
    public User DisplayedUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in", _sharedLocalizer["YouMustSignIn"], _sharedLocalizer["YouMustSignInDescription"], ToastTypes.Error));
        }

        await _database.Entry(currentUser).Collection(u => u.Followers).LoadAsync();

        CurrentUser = currentUser;

        var displayedUser = await _database.User.FindAsync(id);
        if (displayedUser is null) {
            return NotFound();
        }

        DisplayedUser = displayedUser;

        return Page();
    }
}