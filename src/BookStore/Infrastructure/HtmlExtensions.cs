using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Infrastructure
{
    public static class HtmlExtensions
    {
        public static HtmlString DisabledIf(this IHtmlHelper html, bool condition)
        {
            return new HtmlString(condition ? "disabled=\"disabled\"" : "");
        }
    }
}
