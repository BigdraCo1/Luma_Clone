using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Answer
{
    public string Tuid { get; set; } = null!;

    public string? Answer1 { get; set; }

    public string? QuestionTuid { get; set; }

    public string UserTuid { get; set; } = null!;

    public virtual Question? QuestionTu { get; set; }

    public virtual User UserTu { get; set; } = null!;
}
