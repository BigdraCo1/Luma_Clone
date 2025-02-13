using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Utils;

namespace alma.Controllers.Auth;

public class LoginModel : PageModel {
    public string State { get; set; } = default!;

    public IActionResult OnGet() {
        var StateNounce = Tuid.Generate();
        var RedirectTo = HttpContext.Request.Query["next"].ToString();

        if (RedirectTo.Length <= 0) {
            RedirectTo = "/";
        }

        State = Base64UrlTextEncoder.Encode(System.Text.Encoding.UTF8.GetBytes($"{StateNounce}:{RedirectTo}"));
        HttpContext.Response.Cookies.Append("state", State, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return Page();
    }

    // [BindProperty]
    // public User User { get; set; } = default!;

    // public async Task<IActionResult> OnPostAsync() {
    //     if (!ModelState.IsValid) {
    //         return Page();
    //     }

    //     _context.User.Add(User);
    //     await _context.SaveChangesAsync();

    //     return RedirectToPage("./Index");
    // }
}
