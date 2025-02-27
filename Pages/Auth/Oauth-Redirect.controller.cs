using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using alma.Enums;
using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Pages.Auth;

public class OauthRedirectModel(IConfiguration config, IStringLocalizer<OauthRedirectModel> localizer, DatabaseContext database, ISessionService sessionService, HttpClient client) : PageModel {
    private readonly IConfiguration _config = config;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly DatabaseContext _database = database;
    private readonly ISessionService _sessionService = sessionService;
    private readonly HttpClient _client = client;

    public async Task<IActionResult> OnGetAsync() {
        var cookieState = HttpContext.Request.Cookies["state"];
        var queryState = HttpContext.Request.Query["state"].ToString();
        var authorizationCode = HttpContext.Request.Query["code"].ToString();

        if (cookieState is null || queryState is null ||
            cookieState.Length <= 0 || queryState.Length <= 0 ||
            cookieState != queryState) {
            return Redirect(Toast.AppendQueryString("/auth/sign-in", _localizer["InvalidState"], _localizer["InvalidStateDescription"], ToastTypes.Error));
        }

        var state = Base64Url.DecodeToString(cookieState);
        var stateParts = state.Split(':');
        var redirectTo = stateParts[1];

        if (!redirectTo.StartsWith('/')) {
            redirectTo = $"/{redirectTo}";
        }

        var data = new Dictionary<string, string> {
            { "client_id", _config.GetValue<string>("GoogleOAuth:ClientId")! },
            { "client_secret", _config.GetValue<string>("GoogleOAuth:ClientSecret")! },
            { "code", authorizationCode },
            { "redirect_uri", _config.GetValue<string>("GoogleOAuth:RedirectUri")! },
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
        var user = await _database.User.FindAsync(userId);

        if (user is null) {
            var avatarResponse = await _client.GetAsync(userInfo!["picture"].GetString()!);
            var avatar = await avatarResponse.Content.ReadAsByteArrayAsync();
            var avatarType = avatarResponse.Content.Headers.ContentType!.MediaType!;

            var newUser = new User {
                Id = userId,
                Email = userInfo!["email"].GetString()!,
                Name = userInfo!["name"].GetString()!,
                Username = Formatter.Slugify(userInfo!["name"].GetString()!),
                Avatar = avatar,
                AvatarType = avatarType,
                Bio = "",
                CreatedAt = DateTime.Now
            };

            await _database.User.AddAsync(newUser);
            await _database.SaveChangesAsync();

            user = newUser;
        }

        var session = await _sessionService.GenerateAsync(user);

        HttpContext.Response.Cookies.Append("session", session.Token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = session.ExpiresAt.AddYears(1) // Add 1 year extra to track whether user have logged in before or not
                                                    // Even after the session expired on the server side
        });

        return Redirect(Toast.AppendQueryString(redirectTo, _localizer["SignInSuccess"], null, ToastTypes.Success));
    }
}