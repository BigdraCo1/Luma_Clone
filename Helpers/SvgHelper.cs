using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace alma.Helpers
{
    public static class SvgHelper
    {
        public static IHtmlContent InlineSvg(this IHtmlHelper htmlHelper, string svgPath, IWebHostEnvironment env)
        {
            var filePath = Path.Combine(env.WebRootPath, svgPath);
            var svgContent = File.ReadAllText(filePath);
            return new HtmlString(svgContent);
        }
    }
}