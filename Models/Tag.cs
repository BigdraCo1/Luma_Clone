using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Tag
{
    public string Tuid { get; set; } = null!;

    public string? Name { get; set; }

    public virtual ICollection<Event> EventTus { get; set; } = new List<Event>();
}
