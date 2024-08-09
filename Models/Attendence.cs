using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Attendence
{
    public int AttendenceId { get; set; }

    public int? Sabhatrackid { get; set; }

    public int? Userid { get; set; }

    public bool? Ispresent { get; set; }

    public virtual SabhaTrack? Sabhatrack { get; set; }

    public virtual AksharUser? User { get; set; }
}
