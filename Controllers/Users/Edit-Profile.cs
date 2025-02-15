using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace alma.Controllers.Users;

public class EditProfileModel : PageModel {
    public IActionResult OnGet() {
        return Page();
    }

}
