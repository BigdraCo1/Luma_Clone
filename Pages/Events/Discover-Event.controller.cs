using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
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
            try
            {
                // Base query for all event filtering
                var eventsQuery = _database.Event
                    .Include(e => e.Tag)
                    .Include(e => e.Host)
                    .Where(e => e.EndAt > DateTime.Now);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(search))
                {
                    eventsQuery = eventsQuery.Where(e =>
                        e.Name.Contains(search) ||
                        e.Description.Contains(search));
                }

                // Get events for the main listing
                Events = await eventsQuery
                    .OrderByDescending(e => e.StartAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Load participant counts for each event
                foreach (var eventItem in Events)
                {
                    var goingCount = await _database.UserAttendEvent
                        .CountAsync(uae => uae.EventId == eventItem.Id && uae.Status == "GOING");

                    // Load a few participants for display
                    var participants = await _database.UserAttendEvent
                        .Where(uae => uae.EventId == eventItem.Id && uae.Status == "GOING")
                        .Join(
                            _database.User,
                            uae => uae.UserId,
                            user => user.Id,
                            (uae, user) => user
                        )
                        .Take(6)
                        .ToListAsync();

                    // Add participants to collection
                    foreach (var user in participants)
                    {
                        ((List<User>)eventItem.Participants).Add(user);
                    }
                }

                // Get upcoming events sorted by start date
                UpcomingEvents = await _database.Event
                    .Include(e => e.Tag)
                    .Include(e => e.Host)
                    .Where(e => e.EndAt > DateTime.Now)
                    .OrderBy(e => e.StartAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get all tags for filtering
                Tags = await _database.Tag.ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DiscoveryEventModel: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetImageAsync(string tuid)
        {
            if (string.IsNullOrEmpty(tuid))
            {
                return BadRequest("Invalid event ID.");
            }

            try
            {
                var eventItem = await _database.Event
                    .FirstOrDefaultAsync(e => e.Id == tuid);

                if (eventItem == null || eventItem.Image == null || eventItem.ImageType == null)
                {
                    return NotFound("Event or image not found.");
                }

                return File(eventItem.Image, eventItem.ImageType);
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}