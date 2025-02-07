using System;
using System.Collections.Generic;

namespace alma.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }
}