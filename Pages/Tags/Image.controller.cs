using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;

namespace alma.Pages.Tags;

public class TagImageModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public async Task<IActionResult> OnGetAsync(string id) {
        var tag = await _database.Tag.FindAsync(id);
        if (tag is null) {
            return NotFound();
        }

        return File(tag.Image, tag.ImageType);
    }
}
