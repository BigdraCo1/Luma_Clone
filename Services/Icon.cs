using Microsoft.AspNetCore.Html;

using alma.Utils;

namespace alma.Services;

public interface IIconService {
    IHtmlContent Inline(
        string name,
        string classes = "",
        string size = "",
        string fill = "",
        string stroke = "",
        string strokeWidth = "",
        string strokeLinecap = "",
        string strokeLinejoin = ""
    );
}

/// <summary>
/// A service for dealing with icons in pages.
/// </summary>
/// <param name="env"></param>
public class IconService(IWebHostEnvironment env) : IIconService {
    private readonly IWebHostEnvironment _env = env;

    /// <summary>
    /// Embeds an SVG icon as inline element into the page.
    /// </summary>
    /// <param name="name">The name of the icon. This is the file name of the icon, without extension or parent directory.</param>
    /// <param name="classes">Classes to add to the icon, for styling purposes.</param>
    /// <param name="size">Size of the icon, in pixels.</param>
    /// <param name="fill">Fill color of the icon.</param>
    /// <param name="stroke">Stroke color of the icon.</param>
    /// <param name="strokeWidth">Stroke width of the icon.</param>
    /// <param name="strokeLinecap">Stroke linecap style of the icon.</param>
    /// <param name="strokeLinejoin">Stroke linejoin style of the icon.</param>
    /// <returns>HTML string of the icon for inline embedding.</returns>
    public IHtmlContent Inline(
        string name,
        string classes = "",
        string size = "",
        string fill = "",
        string stroke = "",
        string strokeWidth = "",
        string strokeLinecap = "",
        string strokeLinejoin = ""
    ) {
        var fileName = $"{name}.svg";
        var filePath = Path.Combine(_env.WebRootPath, "lib", "lucide", fileName);
        var svg = new Svg(filePath);

        if (classes.Length > 0) {
            var originalClasses = svg.GetProperty("class");
            if (originalClasses.Length <= 0) {
                svg.SetProperty("class", classes);
            } else {
                svg.SetProperty("class", $"{originalClasses} {classes}");
            }
        }
        if (size.Length > 0) {
            svg.SetProperty("width", size);
            svg.SetProperty("height", size);
        }
        if (fill.Length > 0) {
            svg.SetProperty("fill", fill);
        }
        if (stroke.Length > 0) {
            svg.SetProperty("stroke", stroke);
        }
        if (strokeWidth.Length > 0) {
            svg.SetProperty("stroke-width", strokeWidth);
        }
        if (strokeLinecap.Length > 0) {
            svg.SetProperty("stroke-linecap", strokeLinecap);
        }
        if (strokeLinejoin.Length > 0) {
            svg.SetProperty("stroke-linejoin", strokeLinejoin);
        }

        return new HtmlString(svg.ToString());
    }
}
