using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;

namespace alma.Pages.Users;

public class AvatarModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public async Task<IActionResult> OnGetAvatarAsync(string id) {
        var user = await _database.User.FindAsync(id);
        if (user is null) {
            return NotFound();
        }

        return File(user.Avatar, user.AvatarType);
    }
}
