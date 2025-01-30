using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using alma.Data;
using alma.Models;

namespace alma.Pages_Users
{
    public class IndexModel : PageModel
    {
        private readonly alma.Data.RazorPagesUserContext _context;

        public IndexModel(alma.Data.RazorPagesUserContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.User.ToListAsync();
        }
    }
}
