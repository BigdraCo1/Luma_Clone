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
}