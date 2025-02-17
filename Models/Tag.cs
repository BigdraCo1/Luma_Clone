using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Tag {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string NameEN { get; set; }

    [Required]
    public required string NameTH { get; set; }

    [Required]
    public required string DescriptionEN { get; set; }

    [Required]
    public required string DescriptionTH { get; set; }

    [Required]
    public required byte[] Image { get; set; }

    [Required]
    public required string ImageType { get; set; }

    public ICollection<Event> Events { get; } = [];

    public ICollection<User> Followers { get; } = [];
}