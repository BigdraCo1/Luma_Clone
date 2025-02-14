using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventModel = alma.Models.Event;

namespace alma.Pages.Event
{
    public class GetEventByIdModel : PageModel
    {
        private readonly DatabaseContext _context;

        public GetEventByIdModel(DatabaseContext context)
        {
            _context = context;
        }

        public EventModel? EventModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {
            EventModel = await _context.Events.FirstOrDefaultAsync(i => i.Tuid == tuid);

            if (EventModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetImageAsync(string tuid)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(i => i.Tuid == tuid);

            if (eventItem == null || eventItem.Image == null)
            {
                return NotFound();
            }

            return File(eventItem.Image, "image/png");
        }
    }
}
