using alma.Enums;

namespace alma.Utils;

/// <summary>
/// A utility class for dealing with toasts
/// from the backend side.
/// </summary>
public class Toast {

    /// <summary>
    /// Generates a query string for a toast.
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="description">The description to display</param>
    /// <param name="type">Type of the toast, must be one of <see cref="ToastTypes"/></param>
    /// <returns>Query string for the toast, with no '?' at the start or '&amp;' at the end</returns>
    public static string GenerateQueryString(string message, string description, string type) {
        message = UrlEncoding.Encode(message);
        description = UrlEncoding.Encode(description);
        return $"message={message}&description={description}&type={type}";
    }
}
