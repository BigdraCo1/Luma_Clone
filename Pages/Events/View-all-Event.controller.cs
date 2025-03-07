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
    public class ViewAllEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public ViewAllEventModel(DatabaseContext database)
        {
            _database = database;
        }

        public List<Event> AllEvents { get; set; }

        public async Task<IActionResult> OnGetAsync(string search = "")
        {
            var eventsQuery = _database.Event
                .Include(e => e.Tag)
                .Include(e => e.Host)
                .Include(e => e.Participants)
                .Where(e => e.RegistrationEndAt > DateTime.Now && e.Visibility == VisibilityStatus.PUBLIC && e.Participants.Count < e.MaxParticipants);

            if (!string.IsNullOrEmpty(search))
            {
                eventsQuery = eventsQuery.Where(e =>
                                        e.Name.Contains(search) ||
                                        e.Description.Contains(search));
            }

            AllEvents = await eventsQuery.ToListAsync();

            return Page();
        }
    }
}