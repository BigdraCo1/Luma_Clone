using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Question
{
    public string Tuid { get; set; } = null!;

    public string? Question1 { get; set; }

    public bool? Required { get; set; }

    public string? EventTuid { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Event? EventTu { get; set; }
}
