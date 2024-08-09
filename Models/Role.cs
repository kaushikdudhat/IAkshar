using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string? Rolename { get; set; }

    public virtual ICollection<AksharUser> Users { get; set; } = new List<AksharUser>();
}
