using System.Collections.Generic;
using System.Threading.Tasks;
using Swetugg.Web.Models;

namespace Swetugg.Web.Services
{

    public interface IConferenceService
    {
        IEnumerable<Conference> GetConferences();
        Conference GetConferenceBySlug(string slug);

        IEnumerable<Slot> GetSlotsAndSessions(int conferenceId);
        IEnumerable<Session> GetSessions(int conferenceId);
        IEnumerable<Room> GetRooms(int conferenceId);
        IEnumerable<Speaker> GetSpeakers(int conferenceId);
        IEnumerable<Sponsor> GetSponsors(int conferenceId);
        IDictionary<int, IDictionary<string, SpeakerImage>> GetSpeakerImages(int conferenceId);

        Speaker GetSpeakerBySlug(int conferenceId, string slug);
        Session GetSessionBySlug(int conferenceId, string slug);
    }
}