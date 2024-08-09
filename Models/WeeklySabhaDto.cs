namespace iAkshar.Models
{
    public class WeeklySabhaDto
    {
        public string SabhaName { get; set; }
        public DateTime Date { get; set; }
        public string MandalName { get; set; }
        public int SabhaTrackId { get; set; }
        public int TotalCount { get; set; }
        public int PresentCount { get; set; }
    }
}
