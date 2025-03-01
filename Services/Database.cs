using Microsoft.EntityFrameworkCore;

using alma.Models;

namespace alma.Services;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {
    public DbSet<Answer> Answer { get; set; }
    public DbSet<Event> Event { get; set; }
    public DbSet<Question> Question { get; set; }
    public DbSet<Session> Session { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserAttendEvent> UserAttendEvent { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AttendingEvents)
            .WithMany(e => e.Attendees)
            .UsingEntity<UserAttendEvent>();

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Attendees)
            .WithMany(u => u.AttendingEvents)
            .UsingEntity<UserAttendEvent>();

        modelBuilder.Entity<User>()
            .HasMany(u => u.HostedEvents)
            .WithOne(e => e.Host)
            .HasForeignKey(e => e.HostId)
            .IsRequired();

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Host)
            .WithMany(u => u.HostedEvents)
            .HasForeignKey(e => e.HostId)
            .IsRequired();

        // Many-to-Many: Event and Tag
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Tags)
            .WithMany(t => t.Events)
            .UsingEntity<Dictionary<string, object>>(
                "EventTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagsId"),
                j => j.HasOne<Event>().WithMany().HasForeignKey("EventsId"),
                j => j.HasKey("EventsId", "TagsId")
            );

        // Many-to-Many: Tag and User (Followed Tags)
        modelBuilder.Entity<Tag>()
            .HasMany(t => t.Followers)
            .WithMany(u => u.FollowedTags)
            .UsingEntity<Dictionary<string, object>>(
                "TagUser",
                j => j.HasOne<User>().WithMany().HasForeignKey("FollowersId"),
                j => j.HasOne<Tag>().WithMany().HasForeignKey("FollowedTagsId"),
                j => j.HasKey("FollowedTagsId", "FollowersId")
            );

        // Many-to-Many: User and User (Followers/Following)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Followers)
            .WithMany(u => u.Following)
            .UsingEntity<Dictionary<string, object>>(
                "UserUser",
                j => j.HasOne<User>().WithMany().HasForeignKey("FollowersId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("FollowingId"),
                j => j.HasKey("FollowersId", "FollowingId")
            );

        // One-to-Many: User and Event (Host)
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Host)
            .WithMany(u => u.HostedEvents)
            .HasForeignKey(e => e.HostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-Many: User and Event (Attendees)
        modelBuilder.Entity<UserAttendEvent>()
            .HasKey(ue => new { ue.EventId, ue.UserId });

    }
}