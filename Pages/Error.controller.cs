using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace alma.Pages;

[IgnoreAntiforgeryToken(Order = 2000)]
public class ErrorModel : PageModel
{
    public int Code { get; set; } = default!;
    public string Message { get; set; } = default!;

    public IActionResult OnGet(int code)
    {
        Response.StatusCode = code;
        Code = code;
        Message = ReasonPhrases.GetReasonPhrase(code);
        return Page();
    }
}
