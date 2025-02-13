using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;
using alma.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventModel = alma.Models.Event;

namespace alma.Pages.Event
{
    public class IndexModel : PageModel, IEventList
    {
        private readonly DatabaseContext _context;

        public IndexModel(DatabaseContext context)
        {
            _context = context;
        }

        public IList<EventModel> Events { get; set; } = new List<EventModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            Events = await _context.Events.ToListAsync();
            return Page();
        }
    }
}
