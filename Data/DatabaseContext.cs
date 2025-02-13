using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using alma.Models;
using UserModel = alma.Models.User;


namespace alma.Data;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<UserModel> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=database.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("Answer");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.Answer1).HasColumnName("answer");
            entity.Property(e => e.QuestionTuid).HasColumnName("questionTUID");
            entity.Property(e => e.UserTuid).HasColumnName("userTUID");

            entity.HasOne(d => d.QuestionTu).WithMany(p => p.Answers).HasForeignKey(d => d.QuestionTuid);

            entity.HasOne(d => d.UserTu).WithMany(p => p.Answers).HasForeignKey(d => d.UserTuid);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("Event");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.ApprovalType).HasColumnName("approvalType");
            entity.Property(e => e.Created)
                .HasColumnType("DATETIME")
                .HasColumnName("created");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.End)
                .HasColumnType("DATETIME")
                .HasColumnName("end");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.LocationCity).HasColumnName("locationCity");
            entity.Property(e => e.LocationGmapUrl).HasColumnName("locationGmapUrl");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OwnerTuid).HasColumnName("ownerTUID");
            entity.Property(e => e.Publicity).HasColumnName("publicity");
            entity.Property(e => e.RegistrationEnd)
                .HasColumnType("DATETIME")
                .HasColumnName("registrationEnd");
            entity.Property(e => e.RegistrationStart)
                .HasColumnType("DATETIME")
                .HasColumnName("registrationStart");
            entity.Property(e => e.RegistrationStatus).HasColumnName("registrationStatus");
            entity.Property(e => e.Start)
                .HasColumnType("DATETIME")
                .HasColumnName("start");

            entity.HasOne(d => d.OwnerTu).WithMany(p => p.Events).HasForeignKey(d => d.OwnerTuid);

            entity.HasMany(d => d.TagTus).WithMany(p => p.EventTus)
                .UsingEntity<Dictionary<string, object>>(
                    "EventTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("EventTuid", "TagTuid");
                        j.ToTable("EventTags");
                        j.IndexerProperty<string>("EventTuid").HasColumnName("eventTUID");
                        j.IndexerProperty<string>("TagTuid").HasColumnName("tagTUID");
                    });
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("Question");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.EventTuid).HasColumnName("eventTUID");
            entity.Property(e => e.Question1).HasColumnName("question");
            entity.Property(e => e.Required)
                .HasColumnType("BOOLEAN")
                .HasColumnName("required");

            entity.HasOne(d => d.EventTu).WithMany(p => p.Questions).HasForeignKey(d => d.EventTuid);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("Session");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.Expiry)
                .HasColumnType("DATETIME")
                .HasColumnName("expiry");
            entity.Property(e => e.Issued)
                .HasColumnType("DATETIME")
                .HasColumnName("issued");
            entity.Property(e => e.LastUsed)
                .HasColumnType("DATETIME")
                .HasColumnName("lastUsed");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserTuid).HasColumnName("userTUID");

            entity.HasOne(d => d.UserTu).WithMany(p => p.Sessions).HasForeignKey(d => d.UserTuid);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("Tag");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.Tuid);

            entity.ToTable("User");

            entity.Property(e => e.Tuid).HasColumnName("TUID");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Joined)
                .HasColumnType("DATETIME")
                .HasColumnName("joined");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.ProfilePicture).HasColumnName("profilePicture");
            entity.Property(e => e.Socials)
                .HasColumnType("JSON")
                .HasColumnName("socials");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasMany(d => d.EventTus).WithMany(p => p.UserTus)
                .UsingEntity<Dictionary<string, object>>(
                    "UserParticipatesEvent",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("EventTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserTuid", "EventTuid");
                        j.ToTable("UserParticipatesEvent");
                        j.IndexerProperty<string>("UserTuid").HasColumnName("userTUID");
                        j.IndexerProperty<string>("EventTuid").HasColumnName("eventTUID");
                    });

            entity.HasMany(d => d.FollowedTus).WithMany(p => p.FollowerTus)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFollow",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FollowedTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FollowerTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("FollowerTuid", "FollowedTuid");
                        j.ToTable("UserFollows");
                        j.IndexerProperty<string>("FollowerTuid").HasColumnName("followerTUID");
                        j.IndexerProperty<string>("FollowedTuid").HasColumnName("followedTUID");
                    });

            entity.HasMany(d => d.FollowerTus).WithMany(p => p.FollowedTus)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFollow",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FollowerTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FollowedTuid")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("FollowerTuid", "FollowedTuid");
                        j.ToTable("UserFollows");
                        j.IndexerProperty<string>("FollowerTuid").HasColumnName("followerTUID");
                        j.IndexerProperty<string>("FollowedTuid").HasColumnName("followedTUID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
