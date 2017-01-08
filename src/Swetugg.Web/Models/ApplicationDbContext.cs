using System.Configuration;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Configuration = Swetugg.Web.Migrations.Configuration;

namespace Swetugg.Web.Models
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public DbSet<Conference> Conferences { get; set; }

		public DbSet<Speaker> Speakers { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Sponsor> Sponsors { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<Slot> Slots { get; set; }
        public DbSet<RoomSlot> RoomSlots { get; set; }
        public DbSet<ImageType> ImageTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<CfpSpeaker> CfpSpeakers { get; set; }
        public DbSet<CfpSession> CfpSessions { get; set; }

		public static ApplicationDbContext Create()
		{
            IDatabaseInitializer<ApplicationDbContext> strategy;
            switch (ConfigurationManager.AppSettings["Database_Initialize_Strategy"])
            {
                case "CreateDatabaseIfNotExists":
                    strategy = new CreateDatabaseIfNotExists<ApplicationDbContext>();
                    break;
                case "DropCreateDatabaseAlways":
                    strategy = new DropCreateDatabaseAlways<ApplicationDbContext>();
                    break;
                case "DropCreateDatabaseIfModelChanges":
                    strategy = new DropCreateDatabaseIfModelChanges<ApplicationDbContext>();
                    break;
                case "MigrateDatabaseToLatestVersion":
                    strategy = new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>();
                    break;
                default:
                    strategy = new NullDatabaseInitializer<ApplicationDbContext>();
                    break;
            }
            Database.SetInitializer(strategy);
            return new ApplicationDbContext();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<RoomSlot>().HasRequired(rs => rs.Room).WithMany(r => r.RoomSlots).WillCascadeOnDelete(false);
			modelBuilder.Entity<RoomSlot>().HasRequired(rs => rs.Slot).WithMany(s => s.RoomSlots).WillCascadeOnDelete(false);
		    modelBuilder.Entity<RoomSlot>().HasOptional(rs => rs.AssignedSession).WithMany(s => s.RoomSlots);

            modelBuilder.Entity<Speaker>().HasMany(s => s.Images).WithRequired().WillCascadeOnDelete(false);
            
            modelBuilder.Entity<SpeakerImage>().HasRequired(s => s.ImageType).WithMany().WillCascadeOnDelete(false);

			modelBuilder.Entity<RoomSlot>().HasKey(r => new { r.RoomId, r.SlotId });

			modelBuilder.Entity<SessionSpeaker>().HasRequired(s => s.Speaker).WithMany(s => s.Sessions).WillCascadeOnDelete(false);
			modelBuilder.Entity<SessionSpeaker>().HasRequired(s => s.Session).WithMany(s => s.Speakers).WillCascadeOnDelete(false);
			modelBuilder.Entity<SessionSpeaker>().HasKey(s => new { s.SessionId, s.SpeakerId });

		    modelBuilder.Entity<Session>().HasMany(s => s.Tags).WithMany(t => t.Sessions);

			modelBuilder.Entity<Conference>().HasMany(c => c.Rooms).WithRequired().WillCascadeOnDelete(false);
			modelBuilder.Entity<Conference>().HasMany(c => c.Sessions).WithRequired().WillCascadeOnDelete(false);
			modelBuilder.Entity<Conference>().HasMany(c => c.Speakers).WithRequired().WillCascadeOnDelete(false);
			modelBuilder.Entity<Conference>().HasMany(c => c.Slots).WithRequired().WillCascadeOnDelete(false);
			modelBuilder.Entity<Conference>().HasMany(c => c.Sponsors).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Entity<Conference>().HasMany(c => c.ImageTypes).WithRequired().WillCascadeOnDelete(false);

            modelBuilder.Entity<CfpSpeaker>().HasMany(c => c.Sessions).WithRequired(c => c.Speaker).WillCascadeOnDelete(false);
		    modelBuilder.Entity<CfpSpeaker>().HasOptional(c => c.Speaker).WithMany(sp => sp.CfpSpeakers).WillCascadeOnDelete(false);
            modelBuilder.Entity<CfpSession>().HasOptional(c => c.Session).WithMany(se => se.CfpSessions).WillCascadeOnDelete(false);
		}
	}
}