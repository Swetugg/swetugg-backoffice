using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Swetugg.Web.Models;

namespace Swetugg.Web.Services
{
    public class CachedConferenceService : IConferenceService
    {
        private readonly IConferenceService _conferenceService;
        private static readonly int MinutesToCache;

        static CachedConferenceService()
        {
            var minuteConfig = ConfigurationManager.AppSettings["CachedConferenceService.MinutesToCache"];
            if (!int.TryParse(minuteConfig, out MinutesToCache))
            {
                MinutesToCache = 5;
            }
        }

        public CachedConferenceService(IConferenceService conferenceService)
        {
            _conferenceService = conferenceService;
        }

        private T FromCache<T>(string key, Func<T> fetch)
            where T: class
        {
            var httpCache = HttpContext.Current.Cache;
            var cached = (T)httpCache.Get(key);
            if (cached == null)
            {
                cached = fetch();
                httpCache.Add(key, cached, 
                    null, 
                    DateTime.Now.AddMinutes(MinutesToCache), 
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Default, null);
            }
            return cached;
        }

        public IEnumerable<Conference> GetConferences()
        {
            return FromCache("GetConferences",
                () => (_conferenceService.GetConferences()).ToArray());
        }

        public Conference GetConferenceBySlug(string slug)
        {
            return FromCache("GetConferenceBySlug_" + slug,
                () => (_conferenceService.GetConferenceBySlug(slug)));
        }

        public IEnumerable<Slot> GetSlotsAndSessions(int conferenceId)
        {
            return FromCache("GetSlotsAndSessions_" + conferenceId,
                () => (_conferenceService.GetSlotsAndSessions(conferenceId)));
        }

        public IEnumerable<Session> GetSessions(int conferenceId)
        {
            return FromCache("GetSessions_" + conferenceId,
                () => (_conferenceService.GetSessions(conferenceId)));
        }

        public IEnumerable<Room> GetRooms(int conferenceId)
        {
            return FromCache("GetRooms_" + conferenceId,
                () => (_conferenceService.GetRooms(conferenceId)));
        }

        public IEnumerable<Speaker> GetSpeakers(int conferenceId)
        {
            return FromCache("GetSpeakers_" + conferenceId,
                () => (_conferenceService.GetSpeakers(conferenceId)).ToArray());
        }

        public IEnumerable<Sponsor> GetSponsors(int conferenceId)
        {
            return FromCache("GetSponsors_" + conferenceId,
                () => (_conferenceService.GetSponsors(conferenceId)).ToArray());
        }

        public IDictionary<int, IDictionary<string, SpeakerImage>> GetSpeakerImages(int conferenceId)
        {
            return FromCache("GetSpeakerImages_" + conferenceId,
                () => (_conferenceService.GetSpeakerImages(conferenceId)));
        }

        public Speaker GetSpeakerBySlug(int conferenceId, string slug)
        {
            return FromCache("GetSpeakerBySlug_" + conferenceId + "_" + slug,
                () => (_conferenceService.GetSpeakerBySlug(conferenceId, slug)));
        }

        public Session GetSessionBySlug(int conferenceId, string slug)
        {
            return FromCache("GetSessionBySlug_" + conferenceId + "_" + slug,
                () => (_conferenceService.GetSessionBySlug(conferenceId, slug)));
        }
    }
}