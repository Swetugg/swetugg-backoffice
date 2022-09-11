using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Swetugg.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Conference> Conferences { get; set; }

        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<RoomSlot> RoomSlots { get; set; }
        public DbSet<ImageType> ImageTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SessionType> SessionTypes { get; set; }

        public DbSet<CfpSpeaker> CfpSpeakers { get; set; }
        public DbSet<CfpSession> CfpSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RoomSlot>().HasOne(rs => rs.Room).WithMany(r => r.RoomSlots)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<RoomSlot>().HasOne(rs => rs.Slot).WithMany(s => s.RoomSlots)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<RoomSlot>().HasOne(rs => rs.AssignedSession).WithMany(s => s.RoomSlots).IsRequired(false);

            modelBuilder.Entity<Speaker>().HasMany(s => s.Images).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Speaker>().HasMany(s => s.Tags).WithMany(t => t.Speakers);

            modelBuilder.Entity<SpeakerImage>().HasOne(s => s.ImageType).WithMany().OnDelete(DeleteBehavior.Restrict).IsRequired();

            modelBuilder.Entity<Sponsor>().HasMany(s => s.Images).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<SponsorImage>().HasOne(s => s.ImageType).WithMany().OnDelete(DeleteBehavior.Restrict).IsRequired();

            modelBuilder.Entity<RoomSlot>().HasKey(r => new {r.RoomId, r.SlotId});

            modelBuilder.Entity<SessionSpeaker>().HasOne(s => s.Speaker).WithMany(s => s.Sessions)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<SessionSpeaker>().HasOne(s => s.Session).WithMany(s => s.Speakers)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<SessionSpeaker>().HasKey(s => new {s.SessionId, s.SpeakerId});

            modelBuilder.Entity<Session>().HasMany(s => s.Tags).WithMany(t => t.Sessions);
            modelBuilder.Entity<Session>().HasOne(c => c.SessionType).WithMany(se => se.Sessions)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            modelBuilder.Entity<Conference>().HasMany(c => c.Rooms).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.Sessions).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.Speakers).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.Slots).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.Sponsors).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.ImageTypes).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Conference>().HasMany(c => c.SessionTypes).WithOne().OnDelete(DeleteBehavior.Restrict).IsRequired();

            modelBuilder.Entity<CfpSpeaker>().HasMany(c => c.Sessions).WithOne(c => c.Speaker)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<CfpSpeaker>().HasOne(c => c.Speaker).WithMany(sp => sp.CfpSpeakers)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            modelBuilder.Entity<CfpSession>().HasOne(c => c.Session).WithMany(se => se.CfpSessions)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            modelBuilder.Entity<CfpSession>().HasOne(c => c.SessionType).WithMany(se => se.CfpSessions)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        }
    }
}