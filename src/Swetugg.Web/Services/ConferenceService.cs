using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Swetugg.Web.Models;

namespace Swetugg.Web.Services
{
    public class ConferenceService : IConferenceService
	{
		private readonly ApplicationDbContext _dbContext;

		public ConferenceService(ApplicationDbContext dbContext)
		{
			this._dbContext = dbContext;
		}

	    public IEnumerable<Conference> GetConferences()
	    {
	        var conferences = _dbContext.Conferences.OrderByDescending(c => c.Start).ToList();
	        return conferences;
	    }

	    public Conference GetConferenceBySlug(string slug)
	    {
            var conference = _dbContext.Conferences.SingleOrDefault(c => c.Slug == slug);
            return conference;
        }

	    public IEnumerable<Slot> GetSlotsAndSessions(int conferenceId)
		{
			var slotsWithSessions =
					_dbContext.Slots
						.Include(s => s.RoomSlots.Select(rs => rs.AssignedSession).Select(se => se.Speakers.Select(sp => sp.Speaker)))
                        .Include(s => s.RoomSlots.Select(rs => rs.AssignedSession).Select(se => se.Tags))
						.Where(sl => sl.ConferenceId == conferenceId)
						.OrderBy(s => s.Start)
						.ToList();
			return slotsWithSessions;
		}

		public IEnumerable<Session> GetSessions(int conferenceId)
		{
			var sessions =
					_dbContext.Sessions
						.Include(s => s.Speakers.Select(sp => sp.Speaker))
                        .Include(s => s.Tags)
						.Where(sl => sl.ConferenceId == conferenceId)
						.OrderBy(s => s.Priority)
						.ToList();
			return sessions;
		}

		public IEnumerable<Room> GetRooms(int conferenceId)
		{
			var allRooms = _dbContext.Rooms.Where(r => r.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToList();
			return allRooms;
		}

		public IEnumerable<Speaker> GetSpeakers(int conferenceId)
		{
            var random = new System.Random();
			var allSpeakers = _dbContext.Speakers.
                Where(s => s.ConferenceId == conferenceId && s.Published).
                Include(s => s.Images.Select(i => i.ImageType)).ToList().
                OrderBy(s => s.Priority).
                ThenBy(s => random.Next());
			return allSpeakers;
		}

		public IEnumerable<Sponsor> GetSponsors(int conferenceId)
		{
			var allSponsors = _dbContext.Sponsors.Where(s => s.ConferenceId == conferenceId && s.Published).OrderBy(s => s.Priority).ToList();
			return allSponsors;
		}

	    public Speaker GetSpeakerBySlug(int conferenceId, string slug)
	    {
	        var speaker = _dbContext.Speakers.
                Where(s => s.ConferenceId == conferenceId && s.Slug == slug).
                Include(s => s.Images.Select(i => i.ImageType)).
	            Include(s => s.Sessions.Select(se => se.Session).Select(se => se.RoomSlots.Select(rs => rs.Room))).
	            Include(s => s.Sessions.Select(se => se.Session).Select(se => se.RoomSlots.Select(rs => rs.Slot))).SingleOrDefault();
	        
            return speaker;
	    }

	    public Session GetSessionBySlug(int conferenceId, string slug)
	    {
	        var session = _dbContext.Sessions.
	            Where(s => s.ConferenceId == conferenceId && s.Slug == slug).
	            Include(s => s.Speakers.Select(sp => sp.Speaker)).SingleOrDefault();
	        
            return session;
	    }

        public IDictionary<int, IDictionary<string, SpeakerImage>> GetSpeakerImages(int conferenceId)
        {
            var speakers = _dbContext.Speakers.
                Where(s => s.ConferenceId == conferenceId).
                Include(s => s.Images.Select(i => i.ImageType));

            var speakerImages = new Dictionary<int, IDictionary<string, SpeakerImage>>();
            foreach (var speaker in speakers)
            {
                var lookup = new Dictionary<string, SpeakerImage>();
                foreach (var img in speaker.Images)
                {
                    lookup[img.ImageType.Slug] = img;
                }
                speakerImages[speaker.Id] = lookup;
            }
            return speakerImages;
        }
	}
}