using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using alma.Services;
using System.Threading.Tasks;
using alma.Dtos;

[Route("api/[controller]")]
public class NotificationController : Controller
{
    private readonly EmailService _emailService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(EmailService emailService, ILogger<NotificationController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmailNotification([FromBody] EmailRequest emailRequest)
    {
        if (string.IsNullOrEmpty(emailRequest.ToEmail))
        {
            _logger.LogError("Email address is null or empty.");
            return BadRequest("Email address is required.");
        }

        try
        {
            _logger.LogInformation("Sending email to {ToEmail} with subject {Subject}", emailRequest.ToEmail, emailRequest.Subject);

            // dummy
            var icsContent = _emailService.GenerateIcsContent(
                emailRequest.Subject,
                emailRequest.Body,
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2),
                "Online",
                "organizer@example.com",
                emailRequest.ToEmail,
                "ACCEPTED" 
            );

            await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body, icsContent);
            _logger.LogInformation("Email sent successfully to {ToEmail}", emailRequest.ToEmail);
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", emailRequest.ToEmail);
            return StatusCode(500, $"Failed to send email: {ex.Message}");
        }
    }
}