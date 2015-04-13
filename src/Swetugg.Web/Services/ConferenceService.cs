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

	    public async Task<IEnumerable<Conference>> GetConferences()
	    {
	        var conferences = await _dbContext.Conferences.OrderByDescending(c => c.Start).ToListAsync();
	        return conferences;
	    }

	    public async Task<Conference> GetConferenceBySlug(string slug)
	    {
            var conference = await _dbContext.Conferences.SingleOrDefaultAsync(c => c.Slug == slug);
            return conference;
        }

	    public async Task<IEnumerable<Slot>> GetSlotsAndSessions(int conferenceId)
		{
			var slotsWithSessions =
				await
					_dbContext.Slots
						.Include(s => s.RoomSlots.Select(rs => rs.AssignedSession))
						.Where(sl => sl.ConferenceId == conferenceId)
						.OrderBy(s => s.Start)
						.ToListAsync();
			return slotsWithSessions;
		}

		public async Task<IEnumerable<Session>> GetSessions(int conferenceId)
		{
			var sessions =
				await
					_dbContext.Sessions
						.Include(s => s.Speakers.Select(sp => sp.Speaker))
						.Where(sl => sl.ConferenceId == conferenceId)
						.OrderBy(s => s.Priority)
						.ToListAsync();
			return sessions;
		}

		public async Task<IEnumerable<Room>> GetRooms(int conferenceId)
		{
			var allRooms = await _dbContext.Rooms.Where(r => r.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToListAsync();
			return allRooms;
		}

		public async Task<IEnumerable<Speaker>> GetSpeakers(int conferenceId)
		{
			var allSpeakers = await _dbContext.Speakers.Where(s => s.ConferenceId == conferenceId && s.Published).OrderBy(s => s.Priority).ToListAsync();
			return allSpeakers;
		}

		public async Task<IEnumerable<Sponsor>> GetSponsors(int conferenceId)
		{
			var allSponsors = await _dbContext.Sponsors.Where(s => s.ConferenceId == conferenceId && s.Published).OrderBy(s => s.Priority).ToListAsync();
			return allSponsors;
		}

	    public async Task<Speaker> GetSpeakerBySlug(int conferenceId, string slug)
	    {
	        var speaker = await _dbContext.Speakers.
                Where(s => s.ConferenceId == conferenceId && s.Slug == slug).
	            Include(s => s.Sessions.Select(se => se.Session)).SingleOrDefaultAsync();
	        
            return speaker;
	    }

	    public async Task<Session> GetSessionBySlug(int conferenceId, string slug)
	    {
	        var session = await _dbContext.Sessions.
	            Where(s => s.ConferenceId == conferenceId && s.Slug == slug).
	            Include(s => s.Speakers.Select(sp => sp.Speaker)).SingleOrDefaultAsync();
	        
            return session;
	    }
	}
}