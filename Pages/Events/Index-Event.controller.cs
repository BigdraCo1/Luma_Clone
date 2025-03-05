using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace alma.Pages.Events
{
    public class IndexEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public IndexEventModel(DatabaseContext database)
        {
            _database = database;
        }

        public Event currentEvent { get; set; }
        public IList<User> GoingAttendees { get; set; } = new List<User>();
        public IList<User> DisplayAttendees { get; set; } = new List<User>();
        public string IFramSrc { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {
            // Load the event with related data
            currentEvent = await _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(i => i.Id == tuid);

            if (currentEvent == null)
            {
                return NotFound();
            }

            // Filter attendees with "GOING" status and limit to first 6
            GoingAttendees = await _database.UserAttendEvent
                .Where(uae => uae.EventId == tuid && uae.Status == "GOING")
                .Join(
                    _database.User,
                    uae => uae.UserId,
                    user => user.Id,
                    (uae, user) => user
                )
.ToListAsync();

            DisplayAttendees = await _database.UserAttendEvent
    .Where(uae => uae.EventId == tuid && uae.Status == "GOING")
    .Join(
        _database.User,
                uae => uae.UserId,
        user => user.Id,
                    (uae, user) => user
    )
    .Take(6)
    .ToListAsync();

                    return Page();
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