using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, string icsContent = null)
    {
        if (string.IsNullOrEmpty(toEmail))
        {
            throw new ArgumentNullException(nameof(toEmail), "Email address cannot be null or empty.");
        }

        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var fromEmail = smtpSettings["FromEmail"];
        var fromName = smtpSettings["FromName"];
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        if (!string.IsNullOrEmpty(icsContent))
        {
            var attachment = new Attachment(new MemoryStream(Encoding.UTF8.GetBytes(icsContent)), "event.ics");
            mailMessage.Attachments.Add(attachment);
        }

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.EnableSsl = enableSsl;
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    public string GenerateIcsContent(string subject, string description, DateTime startTime, DateTime endTime, string location, string organizerEmail, string attendeeEmail, string attendeeStatus)
    {
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
        icsContent.AppendLine($"SUMMARY:{subject}");
        icsContent.AppendLine($"DESCRIPTION:{description}");
        icsContent.AppendLine($"LOCATION:{location}");
        icsContent.AppendLine($"ORGANIZER;CN={organizerEmail}:mailto:{organizerEmail}");
        icsContent.AppendLine($"ATTENDEE;CUTYPE=INDIVIDUAL;ROLE=REQ-PARTICIPANT;PARTSTAT={attendeeStatus.ToUpper()};CN={attendeeEmail};X-NUM-GUESTS=0:mailto:{attendeeEmail}");
        icsContent.AppendLine("END:VEVENT");
        icsContent.AppendLine("END:VCALENDAR");

        return icsContent.ToString();
    }
}