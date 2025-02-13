using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserModel = alma.Models.User;

namespace alma.Pages.User
{
    public class GetUserByIdModel : PageModel
    {
        private readonly DatabaseContext _context;

        public GetUserByIdModel(DatabaseContext context)
        {
            _context = context;
        }

        public UserModel UserModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string tuid)
        {
            UserModel = await _context.Users.FirstOrDefaultAsync(i => i.Tuid == tuid);

            if (UserModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetImageAsync(string tuid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(i => i.Tuid == tuid);

            if (user == null || user.ProfilePicture == null)
            {
                return NotFound();
            }

            return File(user.ProfilePicture, "image/png");
        }
    }
}