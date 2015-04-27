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

        private async Task<T> FromCache<T>(string key, Func<Task<T>> fetch)
            where T: class
        {
            var httpCache = HttpContext.Current.Cache;
            var cached = (T)httpCache.Get(key);
            if (cached == null)
            {
                cached = (await fetch());
                httpCache.Add(key, cached, 
                    null, 
                    DateTime.Now.AddMinutes(MinutesToCache), 
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Default, null);
            }
            return cached;
        }

        public async Task<IEnumerable<Conference>> GetConferences()
        {
            return await FromCache("GetConferences",
                async () => (await _conferenceService.GetConferences()).ToArray());
        }

        public async Task<Conference> GetConferenceBySlug(string slug)
        {
            return await FromCache("GetConferenceBySlug_" + slug,
                async () => (await _conferenceService.GetConferenceBySlug(slug)));
        }

        public async Task<IEnumerable<Slot>> GetSlotsAndSessions(int conferenceId)
        {
            return await FromCache("GetSlotsAndSessions_" + conferenceId,
                async () => (await _conferenceService.GetSlotsAndSessions(conferenceId)));
        }

        public async Task<IEnumerable<Session>> GetSessions(int conferenceId)
        {
            return await FromCache("GetSessions_" + conferenceId,
                async () => (await _conferenceService.GetSessions(conferenceId)));
        }

        public async Task<IEnumerable<Room>> GetRooms(int conferenceId)
        {
            return await FromCache("GetRooms_" + conferenceId,
                async () => (await _conferenceService.GetRooms(conferenceId)));
        }

        public async Task<IEnumerable<Speaker>> GetSpeakers(int conferenceId)
        {
            return await FromCache("GetSpeakers_" + conferenceId,
                async () => (await _conferenceService.GetSpeakers(conferenceId)).ToArray());
        }

        public async Task<IEnumerable<Sponsor>> GetSponsors(int conferenceId)
        {
            return await FromCache("GetSponsors_" + conferenceId,
                async () => (await _conferenceService.GetSponsors(conferenceId)).ToArray());
        }

        public async Task<Speaker> GetSpeakerBySlug(int conferenceId, string slug)
        {
            return await FromCache("GetSpeakerBySlug_" + conferenceId + "_" + slug,
                async () => (await _conferenceService.GetSpeakerBySlug(conferenceId, slug)));
        }

        public async Task<Session> GetSessionBySlug(int conferenceId, string slug)
        {
            return await FromCache("GetSessionBySlug_" + conferenceId + "_" + slug,
                async () => (await _conferenceService.GetSessionBySlug(conferenceId, slug)));
        }
    }
}