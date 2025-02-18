using System.ComponentModel.DataAnnotations;

namespace alma.Dtos;

public class EmailRequest
{
    [Required]
    public string ToEmail { get; set; }

    public string Subject { get; set; }
    
    public string Body { get; set; }
}