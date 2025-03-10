using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Services;
using alma.Utils;

namespace alma.Pages.Tags;

public class TagImageModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public async Task<IActionResult> OnGetAsync(string id) {
        var tag = await _database.Tag.FindAsync(id);
        if (tag is null) {
            return NotFound();
        }

        var eTag = Etag.Generate(tag.Image);
        Response.Headers.ETag = eTag;
        Response.Headers.CacheControl = "no-cache";

        if (Request.Headers.IfNoneMatch == eTag) {
            return StatusCode(304);
        }

        return File(tag.Image, tag.ImageType);
    }
}
