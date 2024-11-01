using iAkshar.Models;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace iAkshar.Dto
{
    public class ReportDto
    {
    }

    public class MonthlyReport
    {
        public int TotalSabha { get; set; }
        public int AvgStrength { get; set; }
        public int AvgPresent { get; set; }
        public int AvgPresentPer { get; set; }
        public List<SabhaDetailReport> SabhaList { get; set; }
    }

    public class SabhaDetailReport
    {
        public DateTime? Date { get; set; }
        public string Sabha { get; set; }
        public string Type { get; set; }
        public string Mandal { get; set; }
        public int Strength { get; set; }
        public double PresentPer { get; set; }
        public int New { get; set; }
        public string Status { get; set; }
        public int Change { get; set; }
    }
}
