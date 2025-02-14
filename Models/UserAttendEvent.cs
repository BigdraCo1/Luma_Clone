using System.ComponentModel.DataAnnotations;

namespace alma.Models;

public class UserAttendEvent {
    [Required]
    public required string UserId { get; set; }

    [Required]
    public required string EventId { get; set; }

    [Required]
    public required string Status { get; set; }
}
