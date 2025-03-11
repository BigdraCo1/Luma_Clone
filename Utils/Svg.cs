using System.Text.RegularExpressions;

namespace alma.Utils;

/// <summary>
/// A utility class for dealing with SVG files.
/// </summary>
/// <param name="path">The file path for the SVG file</param>
public class Svg(string path) {
    private string SvgContent { get; set; } = File.ReadAllText(path);

    /// <summary>
    /// Get the properties of the SVG file.
    /// </summary>
    /// <returns>Properties of the SVG file.</returns>
    public Dictionary<string, string> GetProperties() {
        var propertiesRegex = new Regex(@"<svg([^>]*)>", RegexOptions.Multiline);
        var propertiesString = propertiesRegex.Match(SvgContent).Groups[1].Value;

        var propertyRegex = new Regex(@"([a-zA-Z-]+)=""([^""]*)""");
        var propertyMatches = propertyRegex.Matches(propertiesString);

        var properties = new Dictionary<string, string>();

        foreach (Match match in propertyMatches) {
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            properties[key] = value;
        }

        return properties;
    }

    /// <summary>
    /// Get a property of the SVG file.
    /// </summary>
    /// <param name="key">The key of the property</param>
    /// <returns>The value of the property</returns>
    public string GetProperty(string key) {
        return GetProperties()[key];
    }

    /// <summary>
    /// Set a property of the SVG file.
    /// </summary>
    /// <param name="key">The key of the property</param>
    /// <param name="value">The value of the property</param>
    /// <returns></returns>
    public void SetProperty(string key, string value) {
        var properties = GetProperties();
        properties[key] = value;

        var propertiesString = "";
        foreach (var property in properties) {
            propertiesString += $"  {property.Key}=\"{property.Value}\"\n";
        }

        var propertiesRegex = new Regex(@"<svg([^>]*)>", RegexOptions.Multiline);
        SvgContent = propertiesRegex.Replace(SvgContent, $"<svg\n{propertiesString}>");
    }

    /// <summary>
    /// Get the SVG content.
    /// </summary>
    /// <returns>SVG content</returns>
    public override string ToString() {
        return SvgContent;
    }

    /// <summary>
    /// Get the minified SVG content.
    /// Note: Minification is not perfect.
    /// </summary>
    /// <returns>Minified SVG content</returns>
    public string ToMinifiedString() {
        var commentRegex = new Regex(@"<!--(.*?)-->", RegexOptions.Multiline);
        return commentRegex.Replace(SvgContent, "").Replace("\r\n", " ").Replace("\n", " ").Replace("  ", "");
    }
}