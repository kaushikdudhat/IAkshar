using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class SabhaType
{
    public int Sabhatypeid { get; set; }

    public string? Sabhatype1 { get; set; }

    public virtual ICollection<Sabha> Sabhas { get; set; } = new List<Sabha>();
}
