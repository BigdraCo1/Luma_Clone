using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;
using alma.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventModel = alma.Models.Event;
using TagModel = alma.Models.Tag;

namespace alma.Pages.Event
{
    public class DiscoveryModel : PageModel, IEventList, ITagList
    {
        private readonly DatabaseContext _context;

        public DiscoveryModel(DatabaseContext context)
        {
            _context = context;
        }

        public IList<EventModel> Events { get; set; } = new List<EventModel>();

        public IList<TagModel> Tags { get; set; } = new List<TagModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            Events = await _context.Events.Include(e => e.TagTus).ToListAsync();
            Tags = await _context.Tags.Include(t => t.EventTus).ToListAsync();
            return Page();
        }

        public Dictionary<string, int> GetTagEventCounts()
        {
            return Tags.ToDictionary(tag => tag.Name, tag => tag.EventTus.Count);
        }
    }
}