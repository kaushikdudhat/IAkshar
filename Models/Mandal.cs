using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Mandal
{
    public int Mandalid { get; set; }

    public string? Mandalname { get; set; }

    public string? Area { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public int? Pradeshid { get; set; }

    public virtual Pradesh? Pradesh { get; set; }

    public virtual ICollection<Sabha> Sabhas { get; set; } = new List<Sabha>();
}
