using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using alma.Services; // Import the namespace where EmailService is located
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

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

            // Generate ICS content
            var icsContent = _emailService.GenerateIcsContent(
                emailRequest.Subject,
                emailRequest.Body,
                DateTime.UtcNow.AddHours(1), // Example start time
                DateTime.UtcNow.AddHours(2), // Example end time
                "Online", // Example location
                "organizer@example.com", // Example organizer email
                emailRequest.ToEmail, // Attendee email
                "ACCEPTED" // Example attendee status
            );

            var htmlContent = $@"
                <html>
                <head>
                    <meta charset='UTF-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                    <title>Event - Invitation</title>
                    <style>
                        body {{
                            margin: 0;
                            padding: 0;
                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                        }}
                        .container {{
                            width: 100%;
                            padding: 20px;
                        }}
                        .content {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        }}
                        .header {{
                            padding: 20px;
                            background-color: #000480;
                            color: white;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 20px;
                            font-weight: 500;
                            color: #329e00;
                        }}
                        .header h2 {{
                            margin: 10px 0 0;
                            font-size: 28px;
                            font-weight: 700;
                            color: white;
                        }}
                        .section {{
                            padding: 20px;
                            border-bottom: 1px solid #eee;
                        }}
                        .section .detail {{
                            margin: 0;
                            color: #666;
                            font-size: 16px;
                            display: flex;
                            align-items: center;
                            flex-direction: row;
                            gap: 10px;
                        }}
                        .section .detail strong {{
                            color: #333;
                        }}
                        .section .detail img {{
                            margin-right: 10px; /* Add space between image and text */
                            vertical-align: middle; /* Center image vertically */
                        }}
                        .footer {{
                            padding: 10px 30px;
                            text-align: center;
                            border-top: 1px solid #eee;
                        }}
                        .footer p {{
                            margin: 0;
                            color: #666;
                            font-size: 14px;
                        }}
                        .cta {{
                            padding: 0 20px 30px;
                            text-align: center;
                        }}
                        .cta a {{
                            display: inline-block;
                            padding: 14px 30px;
                            text-decoration: none;
                            font-weight: 500;
                            border-radius: 6px;
                            font-size: 16px;
                        }}
                        .cta .primary {{
                            background-color: #000480;
                            color: white;
                        }}
                        .cta .secondary {{
                            color: #000480;
                            border: 1px solid #000480;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td align='center'>
                                    <table role='presentation' class='content'>
                                        <!-- Header -->
                                        <tr>
                                            <td class='header'>
                                                <img src='https://ipfs.io/ipfs/bafkreighva3y45p2oc2pw743u6e47haohnsx7yrcssiw4ftd3g7eilwbyq' alt='Green check mark' width='25' height='25'>
                                                <h1>
                                                    You have registered for
                                                </h1>
                                                <h2>{emailRequest.Subject}</h2>
                                            </td>
                                        </tr>
                                        <!-- Date and Location -->
                                        <tr>
                                            <td class='section'>
                                                <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreigg3n5h7q337f5igbevo2qp6jf6yyihpqdvdmcpqrgwlkkukhnyyu' alt='calendar icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong>{currentTime:ddd, MMMM dd}</strong>
                                                                    <br />
                                                                    <span>{currentTime:hh:mm tt} - {currentTime.AddHours(1):hh:mm tt} GMT+7</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class='detail'>
                                                                <img src='https://ipfs.io/ipfs/bafkreid24bbifw2yg4piw6javuvxxd33mhmmhjkq2y6bul2rbaqk5mbxq4' alt='location icon' width='30' height='30'>
                                                                <div class='time-detail'>
                                                                    <strong><a href='#'>Radisson Blu Plaza Hotel</a></strong>
                                                                    <br />
                                                                    <span>Bangkok</span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <!-- Message -->
                                        <tr>
                                            <td class='section'>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    {emailRequest.Body}
                                                </p>
                                                <p style='margin: 0 0 20px; color: #333; font-size: 16px; line-height: 1.6;'>
                                                    ขอบคุณที่เข้าร่วมกับเรา! เตรียมตัวพบกับประสบการณ์สุดพิเศษและช่วงเวลาที่น่าจดจำไปด้วยกัน
                                                </p>
                                            </td>
                                        </tr>
                                        <!-- CTA Button -->
                                        <tr>
                                            <td class='cta'>
                                                <a href='#' class='primary'>หน้ากิจกรรม</a>
                                                <a href='#' class='secondary'>ตั๋วของฉัน</a>
                                            </td>
                                        </tr>
                                        <!-- Footer -->
                                        <tr>
                                            <td class='footer'>
                                                <p>&copy; 2025 Alma. All Rights Reserved.</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, htmlContent, icsContent);
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