using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;

namespace alma.Pages.Events;

public class EventImageModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public async Task<IActionResult> OnGetAsync(string id) {
        var evnt = await _database.Event.FindAsync(id);
        if (evnt is null) {
            return NotFound();
        }

        return File(evnt.Image, evnt.ImageType);
    }
}
