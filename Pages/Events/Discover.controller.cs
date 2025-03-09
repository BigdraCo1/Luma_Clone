using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Events;

public class DiscoverEventsModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public ICollection<Event> Events { get; set; } = default!;
    public ICollection<Tag> Tags { get; set; } = default!;
    public ICollection<Event> FilteredEvents { get; set; } = default!;
    public string? Query { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string? query) {
        Events = await _database.Event.Where(e => e.Visibility == Visibility.Public).ToListAsync();

        Tags = await _database.Tag.Include(t => t.Events).ToListAsync();

        if (query is not null && query != "") {
            Query = query;
            query = query.ToLower();
            FilteredEvents = await _database.Event.Where(e => e.Visibility == Visibility.Public && (e.Name.ToLower().Contains(query) || e.LocationTitle.ToLower().Contains(query) || e.LocationSubtitle.ToLower().Contains(query) || e.Tag.NameEN.ToLower().Contains(query) || e.Tag.NameTH.ToLower().Contains(query))).ToListAsync();
        }
        return Page();
    }
}