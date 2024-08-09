namespace iAkshar.Dto
{
    public class AttendenceDto
    {
        public int? Sabhatrackid { get; set; }

        public int? Userid { get; set; }

        public bool? Ispresent { get; set; }
    }

    public class AttendenceCountDto
    {
        public int Week { get; set; }
        public int Month { get; set; }
        public int year { get; set; }
    }


    public class AbsenceCountDto
    {
        public int A1_10 { get; set; }
        public int A11_20 { get; set; }
        public int A21_30 { get; set; }
    } 
    public class AbsentDto
    {
        public int Count { get; set; }
        public UserDetailDto UserDetail { get; set; }
    }
}
