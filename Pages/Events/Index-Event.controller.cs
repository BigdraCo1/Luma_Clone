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
    public class IndexEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public IndexEventModel(DatabaseContext database)
        {
            _database = database;
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