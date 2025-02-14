using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Contexts;

namespace alma.Controllers.Users;

public class AvatarModel(DatabaseContext context) : PageModel {
    private readonly DatabaseContext _context = context;

    public async Task<IActionResult> OnGetAvatarAsync(string id) {
        var user = await _context.User.FindAsync(id);
        if (user is null) {
            return NotFound();
        }

        return File(user.Avatar, user.AvatarType);
    }
}
