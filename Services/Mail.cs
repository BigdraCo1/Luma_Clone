using System.Net;
using System.Net.Mail;
using System.Text;

using alma.Utils;

namespace alma.Services;

public interface IMailService {
    Task SendEmailAsync(string[] addresses, string subject, string bodyFormatString, Dictionary<string, string> data, string? icsContent = null);
}

public class MailService(IConfiguration config) : IMailService {
    private readonly IConfiguration _config = config;

    public async Task SendEmailAsync(string[] addresses, string subject, string bodyFormatString, Dictionary<string, string> data, string? icsContent = null) {
        var smtpHost = _config.GetValue<string>("Mail:Host")!;
        var smtpPort = _config.GetValue<int>("Mail:Port")!;
        var smtpUser = _config.GetValue<string>("Mail:User")!;
        var smtpPassword = _config.GetValue<string>("Mail:Password")!;
        var smtpFrom = _config.GetValue<string>("Mail:From")!;
        var smtpFromAddress = _config.GetValue<string>("Mail:FromAddress")!;

        var body = Formatter.FormatString(bodyFormatString, data);

        try {
            using var smtpClient = new SmtpClient(smtpHost, smtpPort) {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            using var message = new MailMessage {
                From = new MailAddress(smtpFromAddress, smtpFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var address in addresses) {
                message.To.Add(new MailAddress(address));
            }

            if (!string.IsNullOrEmpty(icsContent)) {
                var attachment = new Attachment(new MemoryStream(Encoding.UTF8.GetBytes(icsContent)), "event.ics", "text/calendar");
                message.Attachments.Add(attachment);
            }

            await smtpClient.SendMailAsync(message);
        } catch (Exception e) {
            Console.WriteLine($"Error sending email: {e.Message}");
        }
    }
}
