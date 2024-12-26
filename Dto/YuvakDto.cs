namespace iAkshar.Dto
{
    public class YuvakDto
    {
        public int YuvakId { get; set; }
        public string? Firstname { get; set; }

        public string? Middlename { get; set; }

        public string? Lastname { get; set; }

        public string? Mobile { get; set; }
        public string? Emailid { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Address { get; set; }

        public int? Pincode { get; set; }

        public bool? Iskaryakarta { get; set; }

        public string FollowupName { get; set; }

        public string ReferenceName { get; set; }
        public int? FollowupBy { get; set; }
        public int? ReferenceBy { get; set; }
        public string Gender { get; set; }
        public DateTime? LastSabhaAttended { get; set; }
        public double Percentage { get; set; }
        public int SabhaId { get; set; }
        public int RoleId { get; set; }
        public string ProfileImagePath { get; set; }
        public string Type { get; set; }
        public bool? IsAmbrish { get; set; }

        public string SabhaName { get; set; }
        public string MandalName { get; set; }
        public string FollowupMob { get; set; }
        public string RefMob { get; set; }
    }

    public class SabhaDDDto
    {
        public int SabhaId { get; set; }
        public string SabhaName { get; set; }

    }
}
