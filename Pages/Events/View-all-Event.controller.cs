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
    public class ViewAllEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public ViewAllEventModel(DatabaseContext database)
        {
            _database = database;
        }

        public Event currentEvent { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {
            currentEvent = await _database.Event.FirstOrDefaultAsync(i => i.Id == tuid);

            if (currentEvent == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}