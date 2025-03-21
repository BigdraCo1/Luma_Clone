using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

public class EventImageModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public async Task<IActionResult> OnGetAsync(string id) {
        var evnt = await _database.Event.FindAsync(id);
        if (evnt is null) {
            return NotFound();
        }

        var eTag = Etag.Generate(evnt.Image);
        Response.Headers.ETag = eTag;
        Response.Headers.CacheControl = "no-cache";

        if (Request.Headers.IfNoneMatch == eTag) {
            return StatusCode(304);
        }

        return File(evnt.Image, evnt.ImageType);
    }
}
