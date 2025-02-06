using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using alma.Data;
using alma.Models;

namespace alma.Controllers.Users;

public class CreateModel : PageModel {
    private readonly RazorPagesUserContext _context;

    public CreateModel(RazorPagesUserContext context) {
        _context = context;
    }

    public IActionResult OnGet() => Page();

    [BindProperty]
    public User User { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid) {
            return Page();
        }

        _context.User.Add(User);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
