using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using alma.Models;
using alma.Services;

namespace alma.Pages.Tags;

public class ViewTagModel(DatabaseContext database, ISessionService sessionService) : PageModel {
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;

    public new User? User { get; set; } = default!;
    public Tag Tag { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");

        if (user is not null) {
            await _database.Entry(user).Collection(u => u.FollowedTags).LoadAsync();
        }

        User = user;

        var tag = await _database.Tag.Include(t => t.Events).Include(t => t.Followers).FirstOrDefaultAsync(t => t.Id == id);
        if (tag == null) {
            return NotFound();
        }

        Tag = tag;

        return Page();
    }
}
