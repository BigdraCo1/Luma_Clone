using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Tag {
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

    public ICollection<Event> Events { get; } = [];

    public ICollection<User> Followers { get; } = [];
}