using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class User {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string PhoneNumber { get; set; }

    [Required]
    public required byte[] Avatar { get; set; }

    [Required]
    public required string AvatarType { get; set; }

    [Required]
    public required string Bio { get; set; }

    public string? InstagramUsername { get; set; }

    public string? TwitterUsername { get; set; }

    public string? YoutubeUsername { get; set; }

    public string? TikTokUsername { get; set; }

    public string? LinkedinHandle { get; set; }

    public string? WebsiteUrl { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    public ICollection<Event> HostedEvents { get; } = [];

    public ICollection<Event> AttendingEvents { get; } = [];

    public ICollection<User> Following { get; } = [];

    public ICollection<User> Followers { get; } = [];

    public ICollection<Tag> FollowedTags { get; } = [];

    public ICollection<Session> Sessions { get; } = [];

    public ICollection<Answer> Answers { get; } = [];
}