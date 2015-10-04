using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Swetugg.Web.Models
{
    public static class ConferenceInfoExtensions
    {
        public static DateTime CurrentTime(this Conference conference)
        {
            // TODO Store conference timezone on conference object
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var now = DateTime.UtcNow;
            if (timeZone != null)
            {
                now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            }
            return now;
        }

        public static bool IsCfpOpen(this Conference conference)
        {
            var today = conference.CurrentTime().Date;
            return conference.CfpStart.HasValue && conference.CfpStart < today &&
                   (!conference.CfpEnd.HasValue || conference.CfpEnd >= today);
        }
    }

    public static class MvcExtensions
    {
        public static SelectList PreAppend(this IEnumerable<SelectListItem> list, string dataTextField, string selectedValue, bool selected = false)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Selected = selected, Text = dataTextField, Value = selectedValue });
            items.AddRange(list);
            return new SelectList(items, "Value", "Text");
        }

        public static SelectList Append(this IEnumerable<SelectListItem> list, string dataTextField, string selectedValue, bool selected = false)
        {
            var items = list.ToList();
            items.Add(new SelectListItem() { Selected = selected, Text = dataTextField, Value = selectedValue });
            return new SelectList(items, "Value", "Text");
        }

        public static SelectList Default(this IEnumerable<SelectListItem> list, string DataTextField, string SelectedValue)
        {
            return list.PreAppend(DataTextField, SelectedValue, true);
        }
    }
}