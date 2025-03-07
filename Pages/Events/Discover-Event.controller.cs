using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
using alma.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alma.Pages.Events
{
    public class DiscoveryEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public DiscoveryEventModel(DatabaseContext database)
        {
            _database = database;
        }

        public IList<Event> Events { get; set; } = new List<Event>();
        public IList<Event> UpcomingEvents { get; set; } = new List<Event>();
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        public async Task<IActionResult> OnGetAsync(string search = "", int page = 1, int pageSize = 10)
        {
            // Base query for all event filtering
            var eventsQuery = _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Include(e => e.Participants)
                .Where(e => e.RegistrationEndAt > DateTime.Now && e.Visibility == VisibilityStatus.PUBLIC && e.Participants.Count < e.MaxParticipants );

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                eventsQuery = eventsQuery.Where(e =>
                                        e.Name.Contains(search) ||
                                        e.Description.Contains(search));
            }

            // Get events for the main listing
            Events = await eventsQuery
                .GroupJoin(
                    _database.UserAttendEvent,
                    e => e.Id,
                    uae => uae.EventId,
                    (e, uaeGroup) => new { Event = e, ParticipantCount = uaeGroup.Count() }
                )
                .OrderByDescending(e => e.ParticipantCount)
                .ThenByDescending(e => e.Event.StartAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => e.Event)
                .ToListAsync();


            // Get upcoming events sorted by start date
            UpcomingEvents = await _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Where(e => e.EndAt > DateTime.Now && e.Visibility == VisibilityStatus.PUBLIC)
                .OrderBy(e => e.StartAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get all tags for filtering
            Tags = await _database.Tag.ToListAsync();

            return Page();

        }
    }
}