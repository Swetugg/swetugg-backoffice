using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

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
        public static DateTime ConvertToUtcDateTime(this Conference conference, DateTime localDateTime)
        {
            // TODO Store conference timezone on conference object
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            if (timeZone != null)
            {
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
            }
            return localDateTime;
        }

        public static DateTime ConvertDateTime(this Conference conference, DateTime utcDateTime)
        {
            // TODO Store conference timezone on conference object
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            
            if (timeZone != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
            return utcDateTime;
        }

        public static CultureInfo GetCultureInfo(this Conference conference)
        {
            return new CultureInfo("sv-SE");
        }

        public static bool IsCfpOpen(this Conference conference)
        {
            var today = conference.CurrentTime().Date;
            return conference.CfpStart.HasValue && conference.CfpStart <= today &&
                   (!conference.CfpEnd.HasValue || conference.CfpEnd >= today);
        }
    }

    public static class PrincipalExtensions
    {
        public static bool IsAllowedToCreateCfp(this IPrincipal user, Conference conference)
        {
            if (conference.IsCfpOpen())
                return true;

            // TODO Only allow for certain conferences.
            if (user != null && user.IsInRole("VipSpeaker"))
            {
                return true;
            }
            return false;
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