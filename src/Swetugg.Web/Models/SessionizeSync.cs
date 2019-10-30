using System;
using System.Collections.Generic;

namespace Swetugg.Web.Models
{
    public class SessionizeSync
    {
        public List<SpeakerSync> Speakers { get; set; }
        public List<SessionSync> Sessions { get; set; }
    }

    public class SpeakerSync
    {
        public Guid SessionizeId { get; set; }
        public string Name { get; set; }
        public bool AllreadyInDatabase { get; internal set; }
    }

    public class SessionSync
    {
        public int SessionizeId { get; set; }
        public string Title { get; set; }
        public bool AllreadyInDatabase { get; internal set; }
    }
}