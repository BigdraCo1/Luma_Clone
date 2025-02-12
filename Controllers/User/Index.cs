using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;
using alma.Interfaces; // Add this using directive
using System.Collections.Generic;
using System.Threading.Tasks;
using UserModel = alma.Models.User; // Alias for the User class

namespace alma.Pages.User
{
    public class IndexModel : PageModel, IUserList
    {
        private readonly DatabaseContext _context;

        public IndexModel(DatabaseContext context)
        {
            _context = context;
        }

        public IList<UserModel> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            return Page();
        }
    }
}