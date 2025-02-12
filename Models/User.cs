using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class User
{
    public string Tuid { get; set; } = null!;

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Username { get; set; }

    public string? Phone { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public string? Bio { get; set; }

    public string? Socials { get; set; }

    public DateTime? Joined { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Event> EventTus { get; set; } = new List<Event>();

    public virtual ICollection<User> FollowedTus { get; set; } = new List<User>();

    public virtual ICollection<User> FollowerTus { get; set; } = new List<User>();
}
