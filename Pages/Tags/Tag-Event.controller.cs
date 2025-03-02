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
    public class TagEventModel : PageModel
    {
        private readonly DatabaseContext _database;

        public TagEventModel(DatabaseContext database)
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
                var tagItem = await _database.Tag
                    .FirstOrDefaultAsync(e => e.Id == tuid);

                if (tagItem == null || tagItem.Image == null || tagItem.ImageType == null)
                {
                    return NotFound("Event or image not found.");
                }

                return File(tagItem.Image, tagItem.ImageType);
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}