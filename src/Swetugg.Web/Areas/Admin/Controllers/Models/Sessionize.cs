using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swetugg.Web.Areas.Admin.Controllers.Models
{

    public class SezzionizeSpeakers
    {
        public SezzionizeSpeaker[] Speakers { get; set; }
    }

    public class SezzionizeSpeaker
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string bio { get; set; }
        public string tagLine { get; set; }
        public string profilePicture { get; set; }
        public SessoinzeSession[] sessions { get; set; }
        public bool isTopSpeaker { get; set; }
        public SezzionizeLink[] links { get; set; }
        public object[] questionAnswers { get; set; }
        public object[] categories { get; set; }
    }

    public class SessionGroup
    {
        public object groupId { get; set; }
        public string groupName { get; set; }
        public SessoinzeSession[] sessions { get; set; }
    }

    public class SessoinzeSession
    {
        public object[] questionAnswers { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public object startsAt { get; set; }
        public object endsAt { get; set; }
        public bool isServiceSession { get; set; }
        public bool isPlenumSession { get; set; }
        public SezzionizeSpeaker[] speakers { get; set; }
        public Category[] categories { get; set; }
        public object roomId { get; set; }
        public object room { get; set; }
    }


    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public Categoryitem[] categoryItems { get; set; }
        public int sort { get; set; }
    }

    public class Categoryitem
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class SezzionizeLink
    {
        public string title { get; set; }
        public string url { get; set; }
        public string linkType { get; set; }
    }

    public class SezzionizeSchedule
    {
        public string Date { get; set; }
        public bool IsDefault { get; set; }
        public List<SezzionizeScheduleRoom> Rooms { get; set; }
    }

    public class SezzionizeScheduleRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SezzionizeScheduleSession> Sessions { get; set; }
    }

    public class SezzionizeScheduleSpeaker
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class SezzionizeScheduleSession
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public bool IsServiceSession { get; set; } //Lunch, Rest, Etc
        public bool IsPlenumSession { get; set; } //Alla salar (men ligger på bara en)
        public List<SezzionizeScheduleSpeaker> Speakers { get; set; }
        public List<object> Categories { get; set; }
        public int RoomId { get; set; }
        public string Room { get; set; }
        public string LiveUrl { get; set; }
        public string RecordingUrl { get; set; }
        public string Status { get; set; }
    }

}