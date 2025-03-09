using System.Globalization;

namespace alma.Utils;

/// <summary>
/// A utility class for formatting strings.
/// </summary>
public class Formatter {

    /// <summary>
    /// Converts a string to a slug.
    /// Does not guarantee unique input => unique output.
    /// </summary>
    /// <param name="input">The string to generate the slug from</param>
    /// <returns></returns>
    public static string Slugify(string input) {
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        var output = "";

        foreach (char c in input) {
            if (allowedChars.Contains(c)) {
                output += c;
            } else if (c == ' ') {
                output += '-';
            }
        }

        if (output.Length == 0) {
            // generate a random slug if the input can't be converted
            return Token.Generate(128);
        }

        return output;
    }

    public static string FormatDate(DateTime date) {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.Calendar = new GregorianCalendar();
        return date.ToString("d MMM yyyy", culture);
    }

    public static string FormatTime(DateTime date) {
        return date.ToString("HH:mm", CultureInfo.CurrentCulture);
    }

    public static string FormatDateTime(DateTime date) {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.Calendar = new GregorianCalendar();
        return date.ToString("d MMM yyyy - HH:mm", culture);
    }

    public static string FormatString(string input, Dictionary<string, string> data) {
        var output = input;
        foreach (var key in data.Keys) {
            output = output.Replace($"{{{key}}}", data[key]);
        }
        return output;
    }
}
