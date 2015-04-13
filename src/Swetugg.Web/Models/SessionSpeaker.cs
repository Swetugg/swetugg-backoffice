using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class SessionSpeaker
    {
        [ForeignKey("Session")]
        public int SessionId { get; set; }
        [ForeignKey("Speaker")]
        public int SpeakerId { get; set; }
        
        public Session Session { get; set; }
        public Speaker Speaker { get; set; }
    }
}