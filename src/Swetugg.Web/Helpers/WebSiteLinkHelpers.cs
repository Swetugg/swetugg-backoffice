namespace Swetugg.Web.Helpers
{
    public static class WebSiteLinkHelpers
    {
        public static string EnsureHttpLink(this string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return link;

            var trimmed = link.Trim();
            if (trimmed.StartsWith("https://") || trimmed.StartsWith("http://"))
                return trimmed;
            if (trimmed.Contains("."))
                return "http://" + trimmed;

            return link;
        }
    }
}