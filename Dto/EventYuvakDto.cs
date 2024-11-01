namespace iAkshar.Dto
{
    public class EventYuvakDto
    {
        public string YuvakName { get; set; }
        public int YuvakId { get; set; }
        public bool? IsPresent { get; set; }
        public int SabhaID { get; set; }
        public DateTime? LastAttending { get; set; }
        public string ProfileImagePath { get; set; }
    }

    public class UserDataDto
    {
        public string YuvakName { get; set; }
        public int YuvakId { get; set; }
        public string MobileNo { get; set; }
        public string address { get; set; }
        public int SabhaId { get; set; }
        //public bool? IsPresent { get; set; }
        public string SabhaName { get; set; }
        public string MandalName { get; set; }
        public string YuvakType { get; set; }
        public string ProfileImagePath { get; set; }
        //public DateTime? LastAttending { get; set; }
    }
}
