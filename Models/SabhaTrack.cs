using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class SabhaTrack
{
    public int Sabhatrackid { get; set; }

    public int? Sabhaid { get; set; }

    public string? Topic { get; set; }

    public DateTime? Date { get; set; }

    public virtual ICollection<Attendence> Attendences { get; set; } = new List<Attendence>();

    public virtual Sabha? Sabha { get; set; }

    public DateTime CreatedDate { get; set; }
}
