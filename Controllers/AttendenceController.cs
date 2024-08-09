using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;
using System.Globalization;
using iAkshar.Common;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public AttendenceController(AskharyatraContext context)
        {
            _context = context;
        }

        [Route("GetAllAttendence/{userId}")]
        [HttpGet]
        public async Task<ActionResult<object>> GetAttendences(int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
                var currentDate = DateTime.Now;
                var currentDayOfWeek = (int)currentDate.DayOfWeek;
                var startOfWeek = currentDate.AddDays(-currentDayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                var query = from sabhatrack in _context.SabhaTracks
                            join sabha in _context.Sabhas on sabhatrack.Sabhaid equals sabha.Sabhaid
                            join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                            where sabhatrack.Date >= startOfWeek && sabhatrack.Date <= endOfWeek && sabha.Sabhaid == user.Sabhaid
                            select new WeeklySabhaDto
                            {
                                SabhaName = sabha.Sabhaname,
                                Date = Convert.ToDateTime(sabhatrack.Date),
                                MandalName = mandal.Mandalname,
                                SabhaTrackId = sabhatrack.Sabhatrackid,
                                TotalCount = 0,
                                PresentCount = 0
                            };
                var result = await query.ToListAsync();
                foreach (var subresult in result)
                {
                    var TotalYuvakList = _context.Attendences.Where(x => x.Sabhatrackid == subresult.SabhaTrackId).ToList();
                    subresult.TotalCount = TotalYuvakList.Count();
                    subresult.PresentCount = TotalYuvakList.Where(x => x.Ispresent == true).Count();
                }
                if (_context.Attendences == null)
                {
                    return NotFound();
                }
                return result;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("getyuvaklistbysabhatrack/{userId}")]
        [HttpGet]
        public async Task<ActionResult<object>> GetYuvakListBySabhaTrack(int userId , int sabhaTrackId)
        {
            try
            {
                List<YuvakAttendenceDto> result = new List<YuvakAttendenceDto>();
                var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

                if (user != null && user.Roleid.HasValue)
                {
                    switch ((CommonEnum.Role)user.Roleid.Value)
                    {
                        case CommonEnum.Role.Karyakarta:
                            var karyakartaQuery = from attendence in _context.Attendences
                                        join yuvak in _context.Users
                                        on attendence.Userid equals user.UserId
                                        where attendence.Sabhatrackid == sabhaTrackId && yuvak.Referenceby == userId
                                        select new YuvakAttendenceDto
                                        {
                                            YuvakId = yuvak.UserId,
                                            YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
                                            IsPresent = attendence.Ispresent,
                                            SabhaTrackId = sabhaTrackId,
                                            YuvakType = Common.Common.GetYuvakType(yuvak),
                                            MobileNo = yuvak.Mobile,
                                        };
                            result.AddRange(karyakartaQuery);
                            break;

                        default:
                            var leaderQuery = from attendence in _context.Attendences
                                                  join yuvak in _context.Users
                                                  on attendence.Userid equals user.UserId
                                                  where attendence.Sabhatrackid == sabhaTrackId
                                                  select new YuvakAttendenceDto
                                                  {
                                                      YuvakId = yuvak.UserId,
                                                      YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
                                                      IsPresent = attendence.Ispresent,
                                                      SabhaTrackId = sabhaTrackId,
                                                      YuvakType = Common.Common.GetYuvakType(yuvak),
                                                      MobileNo = yuvak.Mobile,
                                                  };
                            result.AddRange(leaderQuery);
                            break;
                    }
                }
             
                return result;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("updateyuvakattendence")]
        [HttpPost]
        public async Task<ActionResult<object>> PostYuvakAttendence([FromBody] List<YuvakAttendenceDto> yuvakAttendenceDtoList)
        {
            try
            {
                foreach (var item in yuvakAttendenceDtoList)
                {
                    var yuvakattendencerecord = _context.Attendences.FirstOrDefault(x => x.Userid == item.YuvakId && x.Sabhatrackid == item.SabhaTrackId);
                    if (yuvakattendencerecord == null)
                    {
                        _context.Attendences.Add(new Attendence
                        {
                            Sabhatrackid = item.SabhaTrackId,
                            Userid = item.YuvakId,
                            Ispresent = item.IsPresent,
                        });
                    }
                    else
                    {
                        yuvakattendencerecord.Ispresent = item.IsPresent;
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(yuvakAttendenceDtoList);
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        [Route("generatecurrentweeksabha")]
        [HttpGet]
        public async Task<ActionResult<object>> generateCurrentWeekSabha()
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);

                var result = Enumerable.Range(0, 7).Select(i => new { Date = startOfWeek.AddDays(i), Day = startOfWeek.AddDays(i).DayOfWeek.ToString() }).ToList();

                var sabhaList = _context.Sabhas.ToList();
                foreach (var sabha in sabhaList)
                {
                    SabhaTrack entity = new SabhaTrack();
                    entity.Sabhaid = sabha.Sabhaid;
                    entity.Topic = "HariPrabodham " + sabha.Sabhaname;
                    var sabhaDate = result.FirstOrDefault(x => x.Day == sabha.Sabhaday);
                    entity.Date = sabhaDate?.Date;
                    _context.SabhaTracks.Add(entity);
                    await _context.SaveChangesAsync();

                    var users = _context.Users.Where(x => x.Sabhaid == sabha.Sabhaid).ToList();
                    foreach (var user in users)
                    {
                        Attendence attendence = new Attendence();
                        attendence.Sabhatrackid = entity.Sabhatrackid;
                        attendence.Userid = user.UserId;
                        attendence.Ispresent = false;
                        _context.Attendences.Add(attendence);
                    }
                    await _context.SaveChangesAsync();
                }
                return "Current Week Sabha Inserted.";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}
