using System.Web;

using alma.Enums;

namespace alma.Utils;

/// <summary>
/// A utility class for dealing with toasts
/// from the backend side.
/// </summary>
public class Toast {
    /// <summary>
    /// Appends a toast query string to a URL.
    /// This replaces existing toast query strings.
    /// </summary>
    /// <param name="url">The URL to append to</param>
    /// <param name="message">The message to display</param>
    /// <param name="description">The description to display</param>
    /// <param name="type">Type of the toast, must be one of <see cref="ToastTypes"/></param>
    /// <returns>A new URL with the toast query strings appended</returns>
    public static string AppendQueryString(string url, string message, string? description, string? type) {
        // TODO: Fix whatever this mess is
        var uriBuilder = new UriBuilder {
            Path = url,
            Scheme = "http",
            Host = "localhost"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Set("toast-message", message);
        if (description is not null) query.Set("toast-description", description);
        if (type is not null) query.Set("toast-type", type);
        uriBuilder.Query = query.ToString();
        return uriBuilder.Path + uriBuilder.Query;
    }
}
