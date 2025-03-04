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

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AttendingEvents)
            .WithMany(e => e.Participants)
            .UsingEntity<UserAttendEvent>();

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Participants)
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
    }
}