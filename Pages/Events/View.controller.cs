using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using alma.Models;
using alma.Services;

namespace alma.Pages.Events;

public class ViewEventModel(DatabaseContext database) : PageModel {
    private readonly DatabaseContext _database = database;

    public Event Event { get; set; } = default!;
    public ICollection<User> ApprovedParticipants { get; set; } = default!;
    public bool IsRegistrationOpen { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id) {
        var evnt = await _database.Event
            .Include(e => e.Host)
            .Include(e => e.Tag)
            .Include(e => e.Participants)
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evnt is null) {
            return NotFound();
        }

        Event = evnt;
        ApprovedParticipants = [.. evnt.Participants
            .Where(p => _database.Set<UserParticipatesEvent>()
                .Any(upe => upe.UserId == p.Id && upe.EventId == evnt.Id && upe.Status == "Approved"))];
        IsRegistrationOpen = evnt.RegistrationOpen && evnt.RegistrationStartAt > DateTime.Now && evnt.RegistrationEndAt < DateTime.Now && ApprovedParticipants.Count < evnt.MaxParticipants;

        return Page();
    }
}