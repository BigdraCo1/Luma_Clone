using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using alma.Contexts;
using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Controllers.Auth;

public class OauthRedirectModel(IConfiguration config, DatabaseContext context, ISessionService sessionService, HttpClient client) : PageModel {
    private readonly IConfiguration _config = config;
    private readonly DatabaseContext _context = context;
    private readonly ISessionService _sessionService = sessionService;
    private readonly HttpClient _client = client;

    public async Task<IActionResult> OnGetAsync() {
        var cookieState = HttpContext.Request.Cookies["state"];
        var queryState = HttpContext.Request.Query["state"].ToString();
        var authorizationCode = HttpContext.Request.Query["code"].ToString();

        if (cookieState is null || queryState is null ||
            cookieState.Length <= 0 || queryState.Length <= 0 ||
            cookieState != queryState) {
            var queryString = Toast.GenerateQueryString(
                "Authentication failed: Invalid state",
                "The authentication URL is invalid, please try again.",
                ToastTypes.Error);
            return Redirect($"/auth/login?{queryString}");
        }

        var state = Base64Url.DecodeToString(cookieState);
        var stateParts = state.Split(':');
        var redirectTo = stateParts[1];

        if (!redirectTo.StartsWith('/')) {
            redirectTo = $"/{redirectTo}";
        }

        var data = new Dictionary<string, string> {
            { "client_id", _config.GetSection("GoogleOAuth")["ClientId"]! },
            { "client_secret", _config.GetSection("GoogleOAuth")["ClientSecret"]! },
            { "code", authorizationCode },
            { "redirect_uri", _config.GetSection("GoogleOAuth")["RedirectUri"]! },
            { "grant_type", "authorization_code" }
        };

        var tokenResponse = await _client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(data));
        var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();
        var tokenResponseData = Json.Deserialize<Dictionary<string, JsonElement>>(tokenResponseString);

        var userInfoJwt = tokenResponseData!["id_token"].GetString();
        var userInfoBase64Url = userInfoJwt!.Split('.')[1];
        var userInfoString = Base64Url.DecodeToString(userInfoBase64Url);
        var userInfo = Json.Deserialize<Dictionary<string, JsonElement>>(userInfoString);

        var userId = Tuid.FromIntString(userInfo!["sub"].GetString()!);
        var user = await _context.User.FindAsync(userId);

        var isNewUser = false;

        if (user is null) {
            isNewUser = true;

            var avatarResponse = await _client.GetAsync(userInfo!["picture"].GetString()!);
            var avatar = await avatarResponse.Content.ReadAsByteArrayAsync();
            var avatarType = avatarResponse.Content.Headers.ContentType!.MediaType!;

            var newUser = new User {
                Id = userId,
                Email = userInfo!["email"].GetString()!,
                Name = userInfo!["name"].GetString()!,
                Username = Formatter.Slugify(userInfo!["name"].GetString()!),
                PhoneNumber = "",
                Avatar = avatar,
                AvatarType = avatarType,
                Bio = "",
                CreatedAt = DateTime.Now
            };

            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();

            user = newUser;
        }

        var session = await _sessionService.GenerateAsync(user);

        HttpContext.Response.Cookies.Append("session", session.Token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = session.ExpiresAt.AddYears(1) // Add 1 year extra to track whether user have logged in before or not
                                                    // Even after the session expired on the server side
        });

        if (isNewUser) {
            return Redirect($"/register?next={UrlEncoder.Encode(redirectTo)}");
        }
        return Redirect(redirectTo);
    }
}