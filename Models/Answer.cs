using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Answer {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Text { get; set; }

    public required Question Question { get; set; }

    public required User User { get; set; }
}