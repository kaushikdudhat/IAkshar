namespace iAkshar.Dto
{
    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class BirthDayCountDto
    {
        public int Today { get; set; }
        public int Tomorrow { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
    }

}
