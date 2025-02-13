namespace alma.Models;

public class UserAttendEvent {
    public required string UserId { get; set; }

    public required string EventId { get; set; }

    public required string Status { get; set; }
}