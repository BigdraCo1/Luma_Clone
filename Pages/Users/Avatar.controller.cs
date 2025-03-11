using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;
using alma.Utils;

namespace alma.Pages.Users;

[IgnoreAntiforgeryToken(Order = 2000)]
public class AvatarModel(DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<IActionResult> OnGetAsync(string id) {
        var user = await _database.User.FindAsync(id);
        if (user is null) {
            return NotFound();
        }

        var eTag = Etag.Generate(user.Avatar);
        Response.Headers.ETag = eTag;
        Response.Headers.CacheControl = "no-cache";

        if (Request.Headers.IfNoneMatch == eTag) {
            return StatusCode(304);
        }

        return File(user.Avatar, user.AvatarType);
    }

    public async Task<IActionResult> OnPostAsync([FromBody] UpdateAvatarDto request) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
        if (user is null) {
            return NotFound();
        }

        try {
            var avatar = DataUrl.Parse(request.AvatarDataUrl);
            user.Avatar = avatar.Bytes;
            user.AvatarType = avatar.Type;
        } catch (Exception) {
            return BadRequest("Invalid data URL format.");
        }

        await _database.SaveChangesAsync();

        return new OkResult();
    }
}

public class UpdateAvatarDto {
    public required string AvatarDataUrl { get; set; }
}

