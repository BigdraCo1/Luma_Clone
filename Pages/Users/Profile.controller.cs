using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

public class ProfileModel(IStringLocalizer<ProfileModel> localizer, DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public User CurrentUser { get; set; } = default!;
    public User ViewingUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in", _localizer["YouMustSignIn"], _localizer["YouMustSignInDescription"], ToastTypes.Error));
        }
        CurrentUser = currentUser;
        var viewingUser = await _database.User.FindAsync(id);
        if (viewingUser is null) {
            return NotFound();
        }
        ViewingUser = viewingUser;
        return Page();
    }
}