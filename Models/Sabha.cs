using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Sabha
{
    public int Sabhaid { get; set; }

    public int? Mandalid { get; set; }

    public string? Sabhaname { get; set; }

    public string? Address { get; set; }

    public string? Area { get; set; }

    public string? Sabhaday { get; set; }

    public string? Sabhatime { get; set; }

    public int? Sabhatypeid { get; set; }

    public int? Contactpersonid { get; set; }

    public string? Googlemap { get; set; }

    public virtual AksharUser? Contactperson { get; set; }

    public virtual Mandal? Mandal { get; set; }

    public virtual ICollection<SabhaTrack> SabhaTracks { get; set; } = new List<SabhaTrack>();

    public virtual SabhaType? Sabhatype { get; set; }

    public virtual ICollection<AksharUser> Users { get; set; } = new List<AksharUser>();
}
