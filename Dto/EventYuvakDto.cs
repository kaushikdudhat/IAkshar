namespace iAkshar.Dto
{
    public class EventYuvakDto
    {
        public string YuvakName { get; set; }
        public int YuvakId { get; set; }
        public bool? IsPresent { get; set; }
        public int SabhaID { get; set; }
        public DateTime? LastAttending { get; set; }
    }
}
