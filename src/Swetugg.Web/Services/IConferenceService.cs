using System.Collections.Generic;
using System.Threading.Tasks;
using Swetugg.Web.Models;

namespace Swetugg.Web.Services
{

    public interface IConferenceService
    {
        Task<IEnumerable<Conference>> GetConferences();
        Task<Conference> GetConferenceBySlug(string slug);

        Task<IEnumerable<Slot>> GetSlotsAndSessions(int conferenceId);
        Task<IEnumerable<Session>> GetSessions(int conferenceId);
        Task<IEnumerable<Room>> GetRooms(int conferenceId);
        Task<IEnumerable<Speaker>> GetSpeakers(int conferenceId);
        Task<IEnumerable<Sponsor>> GetSponsors(int conferenceId);
        
        Task<Speaker> GetSpeakerBySlug(int conferenceId, string slug);
        Task<Session> GetSessionBySlug(int conferenceId, string slug);
    }
}