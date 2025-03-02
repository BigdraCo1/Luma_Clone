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
    public required string Visibility { get; set; }

    [Required]
    public required bool RegistrationOpen { get; set; }

    [Required]
    public required DateTime RegistrationStartAt { get; set; }

    [Required]
    public required DateTime RegistrationEndAt { get; set; }

    [Required]
    public required string ApprovalType { get; set; }

    public int? MaxParticipants { get; set; }

    [Required]
    public required string LocationTitle { get; set; }

    [Required]
    public required string LocationSubtitle { get; set; }

    [Required]
    public required string LocationDescription { get; set; }

    [Required]
    public required string LocationGMapUrl { get; set; }

    [Required]
    public required Tag Tag { get; set; }

    [Required]
    public required string HostId { get; set; }

    public required User Host { get; set; }

    public ICollection<User> Attendees { get; } = [];

    public ICollection<Question> Questions { get; } = [];
}