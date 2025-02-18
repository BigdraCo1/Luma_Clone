using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Utils;

namespace alma.Pages.Auth;

public class LoginModel : PageModel {
    public string State { get; set; } = default!;

    public IActionResult OnGet() {
        var stateNounce = Token.Generate(128);
        var redirectTo = HttpContext.Request.Query["next"].ToString();
        redirectTo = UrlEncoder.Decode(redirectTo);

        if (redirectTo.Length <= 0) {
            redirectTo = "/";
        }

        State = Base64Url.Encode($"{stateNounce}:{redirectTo}");
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

    //     _database.User.Add(User);
    //     await _database.SaveChangesAsync();

    //     return RedirectToPage("./Index");
    // }
}
