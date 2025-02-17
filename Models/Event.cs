using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Event {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required byte[] Image { get; set; }

    [Required]
    public required string ImageType { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    [Required]
    public required DateTime StartAt { get; set; }

    [Required]
    public required DateTime EndAt { get; set; }

    [Required]
    public required string Publicity { get; set; }

    [Required]
    public required string RegistrationStatus { get; set; }

    [Required]
    public required DateTime RegistrationStartAt { get; set; }

    [Required]
    public required DateTime RegistrationEndAt { get; set; }

    [Required]
    public required string ApprovalType { get; set; }

    public int? MaxParticipants { get; set; }

    [Required]
    public required string LocationCity { get; set; }

    [Required]
    public required string Location { get; set; }

    [Required]
    public required string LocationGmapsUrl { get; set; }

    [Required]
    public required string HostId { get; set; }

    public required User Host { get; set; }

    public ICollection<User> Attendees { get; } = [];

    public ICollection<Tag> Tags { get; } = [];

    public ICollection<Question> Questions { get; } = [];
}