using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

[IgnoreAntiforgeryToken(Order = 2000)]
public class AvatarModel(DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAvatarAsync(string id) {
        var user = await _database.User.FindAsync(id);
        if (user is null) {
            return NotFound();
        }

        return File(user.Avatar, user.AvatarType);
    }

    public async Task<IActionResult> OnPostUpdateAvatarAsync([FromBody] AvatarUpdateRequest request) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return NotFound();
        }

        var match = Regex.Match(request.AvatarDataUrl, @"data:(?<type>.+?);base64,(?<data>.+)");
        if (!match.Success) {
            return BadRequest("Invalid data URL format.");
        }

        user.AvatarType = match.Groups["type"].Value;
        user.Avatar = Base64.Decode(match.Groups["data"].Value);

        _database.User.Update(user);
        await _database.SaveChangesAsync();

        return new OkResult();
    }
}

public class AvatarUpdateRequest {
    public required string AvatarDataUrl { get; set; }
}

