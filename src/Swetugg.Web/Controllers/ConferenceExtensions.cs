using System;
using System.Text.RegularExpressions;

namespace Swetugg.Web.Controllers
{
    public static class ConferenceExtensions
    {
        public static string Slugify(this string phrase, int maxLength = 250)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            str = Regex.Replace(str, @"-+", "-"); // more than one hyphen in a row, convert to single
            return str;
        }

        public static string Truncate(this string value, int maxChars)
        {
            if (value.Length <= maxChars)
                return value;
            var maxString = value.Substring(0, maxChars);
            var lastSpace = maxString.LastIndexOf(" ", StringComparison.CurrentCulture);
            if (lastSpace > 0)
            {
                maxString = maxString.Substring(0, lastSpace);
            }
            return maxString + " ...";
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}