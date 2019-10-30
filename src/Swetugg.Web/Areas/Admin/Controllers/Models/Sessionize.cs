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

}