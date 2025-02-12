using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Event
{
    public string Tuid { get; set; } = null!;

    public string? Name { get; set; }

    public byte[]? Image { get; set; }

    public string? Description { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public string? Publicity { get; set; }

    public string? RegistrationStatus { get; set; }

    public DateTime? RegistrationStart { get; set; }

    public DateTime? RegistrationEnd { get; set; }

    public string? ApprovalType { get; set; }

    public string? LocationCity { get; set; }

    public string? LocationGmapUrl { get; set; }

    public string? Location { get; set; }

    public string? OwnerTuid { get; set; }

    public virtual User? OwnerTu { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Tag> TagTus { get; set; } = new List<Tag>();

    public virtual ICollection<User> UserTus { get; set; } = new List<User>();
}
