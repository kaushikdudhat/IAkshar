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
    }
}
