using iAkshar.Common;
using iAkshar.Dto;
using iAkshar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public ReportController(AskharyatraContext context)
        {
            _context = context;
        }

        // GET: api/Report
        [HttpGet("BloodGroupCount")]
        public async Task<object> BloodGroupCount()
        {
            var result = await _context.Users.GroupBy(x => x.Bloodgroup).Where(x => x.Key != null).Select(x =>
            new BloodCount
            {
                Blood = x.Key,
                Count = x.Count()
            }).ToListAsync();

            return Common.Common.GenerateSuccResponse(result);
        }

        [HttpGet("BloodGroupList")]
        public async Task<object> BloodGroupCount(string bloodGroup)
        {


            var result = await (from yuvak in _context.Users
                                join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
                                join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                                where yuvak.Bloodgroup == bloodGroup
                                select new UserDataDto
                                {
                                    YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
                                    YuvakId = yuvak.UserId,
                                    SabhaName = sabha.Sabhaname,
                                    SabhaId = sabha.Sabhaid,
                                    MandalName = mandal.Mandalname,
                                    MobileNo = yuvak.Mobile,
                                    address = yuvak.Address,
                                    YuvakType = Common.Common.GetYuvakType(yuvak),
                                    ProfileImagePath = yuvak.ProfileImagePath
                                }).ToListAsync();

            return Common.Common.GenerateSuccResponse(result);

            //foreach (var item in pradeshHeadQuery)
            //{
            //    var lastSabhaRecord = _context.SabhaTracks.Where(x => x.Sabhaid == item.SabhaId).OrderByDescending(x => x.Date).FirstOrDefault();

            //    if (lastSabhaRecord != null)
            //    {
            //        var lastAttendence = _context.Attendences.Where(x => x.Sabhatrackid == lastSabhaRecord.Sabhatrackid && x.Userid == item.YuvakId).FirstOrDefault();
            //        item.IsPresent = lastAttendence == null ? false : lastAttendence.Ispresent.Value;
            //    }
            //}
        }


     
        [HttpGet("WeeklySabha")]
        public async Task<object> WeeklySabha()
        {
            var currentDate = DateTime.Today;
            var firstDateOf12thMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-11);

            MonthlyReport data = new MonthlyReport();

            var sabhaList = _context.Sabhas.ToList();
            data.TotalSabha = sabhaList.Count();
            data.AvgStrength = _context.Users.Count();

            var sabhatTrack = _context.SabhaTracks.Include(x => x.Sabha).ThenInclude(x => x.Mandal).Include(x => x.Sabha.Sabhatype).Where(x => x.Date >= firstDateOf12thMonth).ToList();

            data.SabhaList = new List<SabhaDetailReport>();

            int lastSabhaAttendedCount = 0;
            try
            {

                for (int i = 0; i < sabhatTrack.Count; i++)
                {
                    var sabhaYuvaks = _context.Attendences.Where(x => x.Sabhatrackid == sabhatTrack[i].Sabhatrackid).ToList();

                    int newJoined = _context.Users.Where(x => x.JoiningDate !=null &&  x.JoiningDate <= sabhatTrack[i].Date && x.JoiningDate >= sabhatTrack[i].Date.Value.AddDays(-7) && x.Sabhaid == sabhatTrack[i].Sabhaid).Count();

                    int attended = sabhaYuvaks.Where(x => x.Ispresent == true).Count();
                    data.SabhaList.Add(new SabhaDetailReport()
                    {
                        Date = sabhatTrack[i].Date,
                        Sabha = sabhatTrack[i].Sabha.Sabhaname,
                        Mandal = sabhatTrack[i].Sabha.Mandal.Mandalname,
                        Type = sabhatTrack[i].Sabha.Sabhatype?.Sabhatype1,
                        Strength = attended,
                        Change = attended - lastSabhaAttendedCount,
                        PresentPer = sabhaYuvaks.Count > 0 ? (attended * 100) / sabhaYuvaks.Count : 0,
                        New = newJoined,
                        Status = attended == 0 ? "Done" : "Pending"
                    });
                    lastSabhaAttendedCount = attended;
                }

                var sabhaTrackIds = sabhatTrack.Select(x => x.Sabhatrackid).ToList();
                var attendance = _context.Attendences.Where(x => sabhaTrackIds.Contains(x.Sabhatrackid.Value)).ToList();
                data.AvgStrength = attendance.Where(x => x.Ispresent == true).Count();
            }
            catch (Exception ex)
            {

                throw;
            }
            return Common.Common.GenerateSuccResponse(data);
        }


        [HttpGet("MonthlySabha")]
        public async Task<object> MonthlySabha()
        {
            var currentDate = DateTime.Today;
            var firstDateOf12thMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-11);

            MonthlyReport data = new MonthlyReport();

            var sabhaList = _context.Sabhas.ToList();
            data.TotalSabha = sabhaList.Count();
            data.AvgStrength = _context.Users.Count();

            var sabhatTrack = _context.SabhaTracks.Include(x => x.Sabha).ThenInclude(x => x.Mandal).Include(x => x.Sabha.Sabhatype).Where(x => x.Date >= firstDateOf12thMonth).ToList();

            data.SabhaList = new List<SabhaDetailReport>();

            int lastSabhaAttendedCount = 0;
            try
            {

                for (int i = 0; i < sabhatTrack.Count; i++)
                {
                    var sabhaYuvaks = _context.Attendences.Where(x => x.Sabhatrackid == sabhatTrack[i].Sabhatrackid).ToList();

                    int newJoined = _context.Users.Where(x => x.JoiningDate != null && x.JoiningDate <= sabhatTrack[i].Date && x.JoiningDate >= sabhatTrack[i].Date.Value.AddDays(-7) && x.Sabhaid == sabhatTrack[i].Sabhaid).Count();

                    int attended = sabhaYuvaks.Where(x => x.Ispresent == true).Count();
                    data.SabhaList.Add(new SabhaDetailReport()
                    {
                        Date = sabhatTrack[i].Date,
                        Sabha = sabhatTrack[i].Sabha.Sabhaname,
                        Mandal = sabhatTrack[i].Sabha.Mandal.Mandalname,
                        Type = sabhatTrack[i].Sabha.Sabhatype?.Sabhatype1,
                        Strength = attended,
                        Change = attended - lastSabhaAttendedCount,
                        PresentPer = sabhaYuvaks.Count > 0 ? (attended * 100) / sabhaYuvaks.Count : 0,
                        New = newJoined,
                        Status = attended == 0 ? "Done" : "Pending"
                    });
                    lastSabhaAttendedCount = attended;
                }

                var sabhaTrackIds = sabhatTrack.Select(x => x.Sabhatrackid).ToList();
                var attendance = _context.Attendences.Where(x => sabhaTrackIds.Contains(x.Sabhatrackid.Value)).ToList();
                data.AvgStrength = attendance.Where(x => x.Ispresent == true).Count();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Common.Common.GenerateSuccResponse(data);
        }
    }

}
