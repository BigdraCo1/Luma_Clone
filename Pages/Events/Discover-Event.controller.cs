using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
using System;
using System.Collections.Generic;
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
                if (!string.IsNullOrEmpty(search))
                {
                    Events = await _database.Event
                        .Include(e => e.Tags)
                        .Include(e => e.Host)
                        .Include(e => e.Attendees.Where(u =>
                            _database.UserAttendEvent.Any(uae =>
                                uae.UserId == u.Id &&
                                uae.EventId == e.Id &&
                                uae.Status == "GOING")))
                        .Where(e => e.EndAt > DateTime.Now)
                        .Where(e => e.Name.Contains(search) || e.Description.Contains(search))
                        .OrderByDescending(e => e.Attendees.Count)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }
                else
                {
                    Events = await _database.Event
                        .Include(e => e.Tags)
                        .Include(e => e.Host)
                        .Include(e => e.Attendees.Where(u =>
                            _database.UserAttendEvent.Any(uae =>
                                uae.UserId == u.Id &&
                                uae.EventId == e.Id &&
                                uae.Status == "GOING")))
                        .Where(e => e.EndAt > DateTime.Now)
                        .OrderByDescending(e => e.Attendees.Count)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                UpcomingEvents = await _database.Event
                    .Include(e => e.Tags)
                    .Include(e => e.Host)
                    .Where(e => e.EndAt > DateTime.Now)
                    .OrderBy(e => e.StartAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Tags = await _database.Tag
                    .Include(e => e.Events)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
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