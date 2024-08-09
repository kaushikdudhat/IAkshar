﻿using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class AksharUser
{
    public int UserId { get; set; }

    public string? Firstname { get; set; }

    public string? Middlename { get; set; }

    public string? Lastname { get; set; }

    public string? Mobile { get; set; }

    public string? Password { get; set; }
    public string Gender { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public int? Pincode { get; set; }

    public string? Education { get; set; }

    public string? Educationstatus { get; set; }

    public string? Maritalstatus { get; set; }

    public bool? Isattending { get; set; }

    public bool? Isirregular { get; set; }

    public bool? Ispresent { get; set; }

    public bool? Iskaryakarta { get; set; }

    public int? Followupby { get; set; }

    public int? Referenceby { get; set; }

    public int? Roleid { get; set; }

    public string? Bloodgroup { get; set; }

    public string? Emailid { get; set; }

    public int? Sabhaid { get; set; }

    public DateTime? JoiningDate { get; set; }
    public bool? IsAmbrish { get; set; }
    public string? ProfileImagePath { get; set; }
    public virtual ICollection<Attendence> Attendences { get; set; } = new List<Attendence>();

    public virtual Role? Role { get; set; }

    public virtual Sabha? Sabha { get; set; }

    public virtual ICollection<Sabha> Sabhas { get; set; } = new List<Sabha>();
}