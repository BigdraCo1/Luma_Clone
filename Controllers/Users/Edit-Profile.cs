using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Models;
using alma.Services;

namespace alma.Controllers.Users;

public class EditProfileModel(ISessionService sessionService) : PageModel {
    private readonly ISessionService _sessionService = sessionService;

    public User CurrentUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync() {
        var currentUser = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (currentUser is null) {
            return Redirect("/auth/login?next=/users/edit-profile");
        }
        CurrentUser = currentUser;
        return Page();
    }

}
