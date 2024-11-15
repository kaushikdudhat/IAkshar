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


     
        [HttpGet("SabhaAttendance")]
        public async Task<object> SabhaAttendance(int? sabhaId)
        {
            var currentDate = DateTime.Today;
            var firstDateOf12thMonth = currentDate.AddMonths(-11).AddDays(-currentDate.Day + 1);

            var data = new MonthlyReport
            {
                TotalSabha = _context.Sabhas.Count(),
                AvgStrength = _context.Users.Count() / Math.Max(_context.Sabhas.Count(), 1),  // Avoid divide by zero
                SabhaList = new List<SabhaDetailReport>()
            };

            try
            {
                var sabhaTracks = _context.SabhaTracks
                    .Include(x => x.Sabha)
                        .ThenInclude(x => x.Mandal)
                    .Include(x => x.Sabha.Sabhatype)
                    .Where(x => (sabhaId==null || x.Sabhaid== sabhaId) && x.Date >= firstDateOf12thMonth)
                    .ToList();

                var sabhaTrackIds = sabhaTracks.Select(x => x.Sabhatrackid).ToList();
                var allAttendances = _context.Attendences
                    .Where(x => sabhaTrackIds.Contains(x.Sabhatrackid.Value))
                    .ToList();
                int lastSabhaAttendedCount = 0;
                int? lastSabhaId = 0;
                foreach (var track in sabhaTracks)
                {
                    var sabhaYuvaks = allAttendances
                        .Where(x => x.Sabhatrackid == track.Sabhatrackid)
                        .ToList();

                    int attended = sabhaYuvaks.Count(x => x.Ispresent.Value);
                    int newJoined = _context.Users.Count(x =>
                        x.JoiningDate != null &&
                        x.JoiningDate <= track.Date &&
                        x.JoiningDate >= track.Date.Value.AddDays(-7) &&
                        x.Sabhaid == track.Sabhaid
                    );


                    if (lastSabhaId != track.Sabhaid)
                        lastSabhaAttendedCount = 0;

                    data.SabhaList.Add(new SabhaDetailReport
                    {
                        Date = track.Date,
                        Sabha = track.Sabha.Sabhaname,
                        Mandal = track.Sabha.Mandal.Mandalname,
                        Type = track.Sabha.Sabhatype?.Sabhatype1,
                        Total = sabhaYuvaks.Count,
                        Attended = attended,
                        Change = attended - lastSabhaAttendedCount,
                        PresentPer = sabhaYuvaks.Count > 0 ? (attended * 100) / sabhaYuvaks.Count : 0,
                        New = newJoined,
                        Status = attended == 0 ? "Pending" : "Done"
                    });

                    lastSabhaAttendedCount = attended;
                    lastSabhaId = track.Sabhaid;

                }
            }
            catch (Exception ex)
            {
                return Common.Common.GenerateError(ex.Message);
            }

            return Common.Common.GenerateSuccResponse(data);
        }
    }

}
