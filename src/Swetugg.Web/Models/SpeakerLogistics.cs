namespace Swetugg.Web.Models
{
	public class SpeakerLogistics
	{
		public int Id { get; set; }
		public string Notes { get; set; }
		public bool AttendingDinner { get; set; }
		public bool TwitterList { get; set; }
		public bool AccomodationDone { get; set; }
		public bool TravelDone { get; set; }

		public Speaker Speaker { get; set; }
		public int? SpeakerId { get; set; }
	}
}