using System.Text;

namespace alma.Utils;

public class Ics {
    public static string GenerateIcsString(string name, string description, DateTime startTime, DateTime endTime, string location, string organizerEmail, string attendeeEmail, string attendeeStatus = "ACCEPTED") {
        var icsContent = new StringBuilder();
        icsContent.AppendLine("BEGIN:VCALENDAR");
        icsContent.AppendLine("VERSION:2.0");
        icsContent.AppendLine("CALSCALE:GREGORIAN");
        icsContent.AppendLine("METHOD:REQUEST");
        icsContent.AppendLine("BEGIN:VEVENT");
        icsContent.AppendLine($"UID:{Guid.NewGuid()}");
        icsContent.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
        icsContent.AppendLine($"DTSTART:{startTime:yyyyMMddTHHmmssZ}");
        icsContent.AppendLine($"DTEND:{endTime:yyyyMMddTHHmmssZ}");
        icsContent.AppendLine($"SUMMARY:{name}");
        icsContent.AppendLine($"DESCRIPTION:{description}");
        icsContent.AppendLine($"LOCATION:{location}");
        icsContent.AppendLine($"ORGANIZER;CN={organizerEmail}:mailto:{organizerEmail}");
        icsContent.AppendLine($"ATTENDEE;CUTYPE=INDIVIDUAL;ROLE=REQ-PARTICIPANT;PARTSTAT={attendeeStatus.ToUpper()};CN={attendeeEmail};X-NUM-GUESTS=0:mailto:{attendeeEmail}");
        icsContent.AppendLine("END:VEVENT");
        icsContent.AppendLine("END:VCALENDAR");

        return icsContent.ToString();
    }
}