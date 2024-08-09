using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Pradesh
{
    public int Pradeshid { get; set; }

    public string? Pradeshname { get; set; }

    public virtual ICollection<Mandal> Mandals { get; set; } = new List<Mandal>();
}
