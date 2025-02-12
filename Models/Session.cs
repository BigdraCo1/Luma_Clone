using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Session
{
    public string Tuid { get; set; } = null!;

    public string? Token { get; set; }

    public DateTime? Expiry { get; set; }

    public DateTime? Issued { get; set; }

    public DateTime? LastUsed { get; set; }

    public string? UserTuid { get; set; }

    public virtual User? UserTu { get; set; }
}
