using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Question {
    [Key]
    [Required]
    public required string Id { get; set; }

    [Required]
    public required int Number { get; set; }

    [Required]
    public required string Text { get; set; }

    public required Event Event { get; set; }

    public ICollection<Answer> Answers { get; } = [];
}