using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using alma.Models;
using alma.Services;

namespace alma.Pages.Tags;

public class ViewTagModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public Tag Tag { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var tag = await _database.Tag.Include(t => t.Events).Include(t => t.Followers).FirstOrDefaultAsync(t => t.Id == id);
        if (tag == null) {
            return NotFound();
        }

        Tag = tag;

        return Page();
    }
}
