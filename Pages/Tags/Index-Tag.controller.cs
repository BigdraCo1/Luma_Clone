using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using alma.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace alma.Pages.Tags
{
    public class IndexTagModel : PageModel
    {
        private readonly DatabaseContext _database;
        private readonly ISessionService _sessionService;

        public IndexTagModel(DatabaseContext database, ISessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
        }

        public IList<Event> Events { get; set; } = new List<Event>();
        public Tag currentTag { get; set; }
        public User currentUser { get; set; }
        public bool followed { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {
            var user = await _sessionService.GetUserAsync(HttpContext.Request.Cookies["session"] ?? "");
            if (user is null)
            {
                currentUser = null;
            }
            else
            {
                currentUser = user;
            }

            currentTag = await _database.Tag
                .Include(e => e.Events)
                .Include(e => e.Followers)
                .FirstOrDefaultAsync(i => i.Id == tuid);

            if (currentTag == null)
            {
                return NotFound();
            }

            if (currentUser != null)
            {
                followed = currentTag.Followers.Any(p => p.Id == currentUser.Id);
            }

            return Page();
        }
    }
}