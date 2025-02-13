using System.Text.Json;
using System.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Contexts;
using alma.Models;
using alma.Utils;

namespace alma.Controllers.Auth;

public class OauthRedirectModel(IConfiguration configuration, DatabaseContext context, HttpClient client) : PageModel {
    private readonly IConfiguration _configuration = configuration;
    private readonly DatabaseContext _context = context;
    private readonly HttpClient _client = client;

    public async Task<IActionResult> OnGetAsync() {
        var CookieState = HttpContext.Request.Cookies["state"];
        var QueryState = HttpContext.Request.Query["state"].ToString();
        var AuthorizationCode = HttpContext.Request.Query["code"].ToString();

        if (CookieState is null || QueryState is null ||
            CookieState.Length <= 0 || QueryState.Length <= 0 ||
            CookieState != QueryState) {
            return Redirect($"/error?message={HttpUtility.UrlEncode("Invalid state.")}");
        }

        var State = System.Text.Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(CookieState));
        var StateParts = State.Split(':');
        var RedirectTo = StateParts[1];

        if (!RedirectTo.StartsWith('/')) {
            RedirectTo = $"/{RedirectTo}";
        }

        var Data = new Dictionary<string, string> {
            { "client_id", _configuration.GetSection("GoogleOAuth")["ClientId"]! },
            { "client_secret", _configuration.GetSection("GoogleOAuth")["ClientSecret"]! },
            { "code", AuthorizationCode },
            { "redirect_uri", _configuration.GetSection("GoogleOAuth")["RedirectUri"]! },
            { "grant_type", "authorization_code" }
        };

        var TokenResponse = await _client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(Data));
        var TokenResponseString = await TokenResponse.Content.ReadAsStringAsync();
        var TokenResponseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(TokenResponseString);

        var UserInfoJwt = TokenResponseData!["id_token"].GetString();
        var UserInfoBase64 = UserInfoJwt!.Split('.')[1];
        var UserInfoString = System.Text.Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(UserInfoBase64));
        var UserInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(UserInfoString);

        var UserId = Tuid.GetFromIntString(UserInfo!["sub"].GetString()!);

        var User = await _context.User.FindAsync(UserId);

        // TODO: Implement session management

        if (User is not null) {
            return Redirect(RedirectTo);
        }

        var profilePictureResponse = await _client.GetAsync(UserInfo!["picture"].GetString()!);
        var profilePicture = await profilePictureResponse.Content.ReadAsByteArrayAsync();

        var NewUser = new User {
            Id = UserId,
            Email = UserInfo!["email"].GetString()!,
            Name = UserInfo!["name"].GetString()!,
            Username = Formatter.Slugify(UserInfo!["name"].GetString()!),
            ProfilePicture = profilePicture,
            CreatedAt = DateTime.Now
        };

        await _context.User.AddAsync(NewUser);
        await _context.SaveChangesAsync();

        return Redirect($"/register?next={HttpUtility.UrlEncode(RedirectTo)}");
    }
}