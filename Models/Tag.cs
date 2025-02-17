using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Tag {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public ICollection<Event> Events { get; } = [];
}