using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Social {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Platform { get; set; }

    [Required]
    public required string Url { get; set; }

    public required User User { get; set; }
}