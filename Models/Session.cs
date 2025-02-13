using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class Session {
    [Key]
    [Required]
    public required string Id { get; set; }

    public required User User { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    public required DateTime ExpiresAt { get; set; }

    [Required]
    public required DateTime IssuedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }
}