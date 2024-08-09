using iAkshar.Dto;
using iAkshar.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iAkshar.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public DashboardController(AskharyatraContext context)
        {
            _context = context;
        }


        [Route("getBirthdayCount")]
        [HttpGet]
        public async Task<ActionResult<BirthDayCountDto>> getBirthdayCount(int sabhaId, int userId)
        {
            try
            {

                DateTime today = DateTime.UtcNow.Date; // Use DateTime.Now.Date if you want local time
                DateTime tomorrow = today.AddDays(1);
                DateTime startOfWeek = today.AddDays((-(int)today.DayOfWeek) + 1);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                int currentMonth = today.Month;

                IQueryable<AksharUser> query = await getRoleWiseYuvak(sabhaId, userId);

                query = query.Where(x => x.BirthDate != null);
                var todayBirthdays = query.Where(p => p.BirthDate.Value.Month == today.Month && p.BirthDate.Value.Day == today.Day).Count();
                var tomorrowBirthdays = query.Where(p => p.BirthDate.Value.Month == tomorrow.Month && p.BirthDate.Value.Day == tomorrow.Day).Count();
                var weekBirthdays = query.Where(p => (p.BirthDate.Value.Month == startOfWeek.Month && p.BirthDate.Value.Day >= startOfWeek.Day && p.BirthDate.Value.Day <= endOfWeek.Day)
                             || (p.BirthDate.Value.Month == endOfWeek.Month && p.BirthDate.Value.Day >= startOfWeek.Day && p.BirthDate.Value.Day <= endOfWeek.Day)).Count();
                var monthBirthdays = query.Where(p => p.BirthDate.Value.Month == currentMonth).Count();

                var data = new BirthDayCountDto() { Today = todayBirthdays, Tomorrow = tomorrowBirthdays, Week = weekBirthdays, Month = monthBirthdays };
                return Ok(data);

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private async Task<IQueryable<AksharUser>> getRoleWiseYuvak(int sabhaId, int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            IQueryable<AksharUser> query = _context.Users;

            if (user != null && user.Roleid.HasValue)
            {
                switch ((CommonEnum.Role)user.Roleid.Value)
                {
                    case CommonEnum.Role.Karyakarta:
                        query = from yuvak in query
                                where yuvak.Sabhaid == sabhaId &&
                                      yuvak.Referenceby == userId
                                select yuvak;
                        break;
                    case CommonEnum.Role.Leader:
                        query = from yuvak in query
                                where yuvak.Sabhaid == sabhaId
                                select yuvak;
                        break;
                    case CommonEnum.Role.MandalHead:
                        var mandalId = user.Sabha?.Mandalid;
                        if (mandalId.HasValue)
                        {
                            query = from yuvak in query
                                    join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
                                    join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                                    where sabha.Mandalid == mandalId.Value
                                    select yuvak;
                        }
                        break;
                    case CommonEnum.Role.PradeshHead:
                        var city = user.Sabha?.Mandal?.City;
                        if (city != null)
                        {
                            query = from yuvak in query
                                    join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
                                    join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                                    where mandal.City == city
                                    select yuvak;
                        }
                        break;
                }
            }

            return query;
        }

        private IQueryable<AksharUser> getRoleWiseYuvakForJoinee(int sabhaId, int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            IQueryable<AksharUser> query = _context.Users;

            if (user != null && user.Roleid.HasValue)
            {
                switch ((CommonEnum.Role)user.Roleid.Value)
                {
                    case CommonEnum.Role.Leader:
                    case CommonEnum.Role.Karyakarta:
                        query = from yuvak in query
                                where yuvak.Sabhaid == sabhaId
                                select yuvak;
                        break;
                    case CommonEnum.Role.MandalHead:
                        var mandalId = user.Sabha?.Mandalid;
                        if (mandalId.HasValue)
                        {
                            query = from yuvak in query
                                    join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
                                    join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                                    where sabha.Mandalid == mandalId.Value
                                    select yuvak;
                        }
                        break;
                    case CommonEnum.Role.PradeshHead:
                        var city = user.Sabha?.Mandal?.City;
                        if (city != null)
                        {
                            query = from yuvak in query
                                    join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
                                    join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
                                    where mandal.City == city
                                    select yuvak;
                        }
                        break;
                }
            }

            return query;
        }


        [Route("getBirthdayList")]
        [HttpGet]
        public async Task<ActionResult<object>> getBirthdayList(int sabhaId, int userId, string birthDayTime)
        {
            try
            {

                DateTime today = DateTime.UtcNow.Date; // Use DateTime.Now.Date if you want local time
                DateTime tomorrow = today.AddDays(1);

                DateTime startOfCurrentWeek = today;
                DateTime startOfWeek = today.AddDays((-(int)today.DayOfWeek) + 1);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                int currentMonth = today.Month;

                IQueryable<AksharUser> query = await getRoleWiseYuvak(sabhaId, userId);
                query = query.Where(x => x.BirthDate != null);
                switch (birthDayTime)
                {
                    case "Today":
                        query = query.Where(p => p.BirthDate.Value.Month == today.Month && p.BirthDate.Value.Day == today.Day);
                        break;
                    case "Tomorrow":
                        query = query.Where(p => p.BirthDate.Value.Month == tomorrow.Month && p.BirthDate.Value.Day == tomorrow.Day);
                        break;
                    case "Week":
                        query = query.Where(p => (p.BirthDate.Value.Month == startOfWeek.Month && p.BirthDate.Value.Day >= startOfWeek.Day)
                        || (p.BirthDate.Value.Month == endOfWeek.Month && p.BirthDate.Value.Day <= endOfWeek.Day));
                        break;
                    case "Month":
                        query = query.Where(p => p.BirthDate.Value.Month == currentMonth);
                        break;
                }

                return await (from user in query
                              join sabha in _context.Sabhas
                              on user.Sabhaid equals sabha.Sabhaid
                              select new UserDetailDto
                              {
                                  UserId = user.UserId,
                                  Name = user.Firstname + " " + user.Lastname,
                                  Type = user.Iskaryakarta == true ? " Karyakarta" : "Yuvak",
                                  Email = user.Emailid,
                                  MobileNo = user.Mobile,
                                  BirthDate = user.BirthDate ?? user.BirthDate.Value
                              }).ToListAsync();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }




        [Route("GetSabhaCount")]
        [HttpGet]
        public async Task<ActionResult<object>> GetSabhaCount()
        {
            try
            {
                DateTime today = DateTime.UtcNow.Date;
                DateTime tomorrow = today.AddDays(1);
                int currentMonth = today.Month;

                var todayCount = _context.SabhaTracks.Where(p => p.Date != null && p.Date.Value.Month == today.Month && p.Date.Value.Day == today.Day).Count();
                var tomorrowCount = _context.SabhaTracks.Where(p => p.Date != null && p.Date.Value.Month == tomorrow.Month && p.Date.Value.Day == tomorrow.Day).Count();

                return new BirthDayCountDto() { Today = todayCount, Tomorrow = tomorrowCount };
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("GetSabhaList")]
        [HttpGet]
        public async Task<ActionResult<object>> GetSabhaList(string time)
        {
            try
            {

                DateTime today = DateTime.UtcNow.Date;
                DateTime tomorrow = today.AddDays(1);
                int currentMonth = today.Month;

                List<SabhaTrack> query = new List<SabhaTrack>();

                switch (time)
                {
                    case "Today":
                        query = _context.SabhaTracks
                            .Include(x => x.Sabha)
                                .ThenInclude(x => x.Sabhatype)
                            .Include(x => x.Sabha)
                                .ThenInclude(x => x.Mandal)
                            .Where(p => p.Date != null && p.Date.Value.Month == today.Month && p.Date.Value.Day == today.Day).ToList();
                        break;
                    case "Tomorrow":
                        query = _context.SabhaTracks
                            .Include(x => x.Sabha)
                                .ThenInclude(x => x.Sabhatype)
                            .Include(x => x.Sabha)
                                .ThenInclude(x => x.Mandal)
                                .Where(p => p.Date != null && p.Date.Value.Month == tomorrow.Month && p.Date.Value.Day == tomorrow.Day).ToList();
                        break;
                }

                return (from st in query
                            //join sabha in _context.Sabhas
                            //on st.Sabhaid equals sabha.Sabhaid
                        select new SabhaDashDto
                        {
                            Address = st.Sabha.Address,
                            Area = st.Sabha.Area,
                            Sabhaday = st.Sabha.Sabhaday,
                            Mandal = st.Sabha.Mandal?.Mandalname,
                            Googlemap = st.Sabha.Googlemap,
                            Sabhaname = st.Sabha.Sabhaname,
                            Contactpersonid = st.Sabha.Contactpersonid,
                            Sabhatime = st.Sabha.Sabhatime,
                            Sabhatype = st.Sabha.Sabhatype?.Sabhatype1,
                        }).ToList();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("GetNewJoineeCount")]
        [HttpGet]
        public async Task<ActionResult<object>> GetNewJoineeCount(int sabhaId, int userId)
        {
            try
            {

                DateTime today = DateTime.UtcNow.Date;
                DateTime tomorrow = today.AddDays(1);
                DateTime startOfWeek = today.AddDays((-(int)today.DayOfWeek) + 1);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                int currentMonth = today.Month;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                IQueryable<AksharUser> query = getRoleWiseYuvakForJoinee(sabhaId, userId);

                var todayCount = query.Where(p => p.JoiningDate.Value.Month == today.Month && p.JoiningDate.Value.Day == today.Day).Count();
                var tomorrowCount = query.Where(p => p.JoiningDate.Value.Month == tomorrow.Month && p.JoiningDate.Value.Day == tomorrow.Day).Count();


                var weekCount = query.Where(p => p.JoiningDate >= startOfWeek && p.JoiningDate <= endOfWeek).Count();
                var monthCount = query.Where(p => p.JoiningDate >= firstDayOfMonth && p.JoiningDate <= lastDayOfMonth).Count();

                return new BirthDayCountDto() { Today = todayCount, Tomorrow = tomorrowCount, Week = weekCount, Month = monthCount };
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        [Route("GetNewJoineeList")]
        [HttpGet]
        public async Task<ActionResult<object>> GetNewJoineeList(int sabhaId, int userId, string time)
        {
            try
            {
                DateTime today = DateTime.UtcNow.Date;
                DateTime tomorrow = today.AddDays(1);
                DateTime startOfWeek = today.AddDays((-(int)today.DayOfWeek) + 1);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                int currentMonth = today.Month;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                IQueryable<AksharUser> query = getRoleWiseYuvakForJoinee(sabhaId, userId);

                switch (time)
                {
                    case "Today":
                        query = query.Where(p => p.JoiningDate.Value.Month == today.Month && p.JoiningDate.Value.Day == today.Day);
                        break;
                    case "Tomorrow":
                        query = query.Where(p => p.JoiningDate.Value.Month == tomorrow.Month && p.JoiningDate.Value.Day == tomorrow.Day);
                        break;
                    case "Week":
                        query = query.Where(p => p.JoiningDate.Value.Month == today.Month && p.JoiningDate.Value.Day == today.Day);
                        break;
                    case "Month":
                        query = query.Where(p => p.JoiningDate >= firstDayOfMonth && p.JoiningDate <= lastDayOfMonth);
                        break;
                    default:
                        query = query.Where(p => p.JoiningDate.Value.Month == today.Month && p.JoiningDate.Value.Day == today.Day);
                        break;
                }

                return await (from user in query
                              join sabha in _context.Sabhas
                              on user.Sabhaid equals sabha.Sabhaid
                              select new UserDetailDto
                              {
                                  UserId = user.UserId,
                                  Name = user.Firstname + " " + user.Lastname,
                                  Type = Common.Common.GetYuvakType(user),
                                  Email = user.Emailid,
                                  MobileNo = user.Mobile,
                                  BirthDate = user.BirthDate
                              }).ToListAsync();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("GetAttendanceList")]
        [HttpGet]
        public async Task<ActionResult<object>> GetAttendanceList(int sabhaId, int userId, string time)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound("User not found.");

                var currentDate = DateTime.Now;
                DateTime startDate, endDate;

                switch (time)
                {
                    case "Week":
                        startDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                        endDate = startDate.AddDays(7).AddSeconds(-1);
                        break;
                    case "Month":
                        startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        endDate = startDate.AddMonths(1).AddSeconds(-1);
                        break;
                    case "Year":
                        startDate = new DateTime(currentDate.Year, 1, 1);
                        endDate = startDate.AddYears(1).AddSeconds(-1);
                        break;
                    default:
                        return BadRequest("Invalid time period specified.");
                }

                var query = getRoleWiseYuvakForJoinee(sabhaId, userId);

                var userIds = await (from au in query
                                     join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                     join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid
                                     where st.Date >= startDate && st.Date <= endDate && att.Ispresent == true
                                     select att.Userid).Distinct().ToListAsync();

                var userDetails = await _context.Users
                                                .Where(x => userIds.Contains(x.UserId))
                                                .Select(q => new UserDetailDto
                                                {
                                                    UserId = q.UserId,
                                                    Name = q.Firstname + " " + q.Lastname,
                                                    Type = Common.Common.GetYuvakType(user),
                                                    Email = q.Emailid,
                                                    MobileNo = q.Mobile
                                                })
                                                .ToListAsync();

                return userDetails;
            }
            catch (Exception e)
            {
                return StatusCode(500, e.ToString());
            }
        }


        [Route("GetAttendanceCount")]
        [HttpGet]
        public async Task<ActionResult<object>> GetAttendanceCount(int sabhaId, int userId)
        {
            try
            {

                var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
                var currentDate = DateTime.Now;
                var currentDayOfWeek = (int)currentDate.DayOfWeek;
                var startOfWeek = currentDate.AddDays(-currentDayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

                DateTime startOfYear = new DateTime(currentDate.Year, 1, 1);
                DateTime endOfYear = startOfYear.AddYears(1).AddSeconds(-1);

                IQueryable<AksharUser> query = getRoleWiseYuvakForJoinee(sabhaId, userId);


                var weekCount = await (from au in query
                                       join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                       join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid
                                       where st.Date >= startOfWeek && st.Date <= endOfWeek && att.Ispresent == true
                                       select att.Userid).Distinct().CountAsync();

                var monthCount = await (from au in query
                                        join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                        join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid
                                        where st.Date >= startOfMonth && st.Date <= endOfMonth && att.Ispresent == true
                                        //group au by new { au.UserId, au.Sabhaid } into grouped
                                        select att.Userid).Distinct().CountAsync();

                var yearCount = await (from au in query
                                       join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                       join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid
                                       where st.Date >= startOfYear && st.Date <= endOfYear && att.Ispresent == true
                                       //group au by new { au.UserId, au.Sabhaid } into grouped
                                       select att.Userid).Distinct().CountAsync();

                return new AttendenceCountDto { Week = weekCount, Month = monthCount, year = yearCount };
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [Route("GetAbsentList")]
        [HttpGet]
        public async Task<ActionResult<object>> GetAbsentList(int sabhaId, int userId, string type)
        {
            try
            {

                var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

                IQueryable<AksharUser> query = await getRoleWiseYuvak(sabhaId, userId);
                int fromCount = 1;
                int toCount = 10;

                switch (type)
                {
                    case "A11_20":
                        fromCount = 11;
                        toCount = 20;
                        break;
                    case "A21_30":
                        fromCount = 21;
                        toCount = 30;
                        break;
                }

                //List<AbsentDto> result = await (from au in query
                //                                join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid into auSt
                //                                from st in auSt.DefaultIfEmpty()
                //                                join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid into stAtt
                //                                from att in stAtt.DefaultIfEmpty()
                //                                where att == null
                //                                group au by au.UserId into groupedUsers
                //                                where groupedUsers.Count() >= fromCount && groupedUsers.Count() <= toCount
                //                                select new AbsentDto
                //                                {
                //                                    Count = groupedUsers.Count(),
                //                                    UserDetail = new UserDetailDto
                //                                    {
                //                                        Email = groupedUsers.First().Emailid,
                //                                        MobileNo = groupedUsers.First().Mobile,
                //                                        Name = groupedUsers.First().Firstname + " " + groupedUsers.First().Lastname,
                //                                        Type = (groupedUsers.First().Iskaryakarta == true ? "Karyakarta" : "Yuvak"),
                //                                        UserId = groupedUsers.First().UserId
                //                                    }
                //                                }).ToListAsync();

                var absentData = (from au in query
                                  join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                  join att in _context.Attendences on new { x1 = st.Sabhatrackid, x2 = au.UserId } equals new { x1 = att.Sabhatrackid.Value, x2 = att.Userid.Value }
                                  where att.Ispresent == false
                                  group au by au.UserId into g
                                  select new
                                  {
                                      userData = g.FirstOrDefault(),
                                      AbsentCount = g.Count()
                                  }).ToList();

                if (absentData.Count() > 0)
                {
                    var data = absentData.Where(x => x.AbsentCount >= fromCount && x.AbsentCount <= toCount).ToList();

                    return data.Select(x => new UserDetailDto
                    {
                        Email = x.userData.Emailid,
                        MobileNo = x.userData.Mobile,
                        Name = x.userData.Firstname + " " + x.userData.Lastname,
                        Type = (x.userData.Iskaryakarta == true ? "Karyakarta" : "Yuvak"),
                        UserId = x.userData.UserId
                    }).ToList();

                }


                return new UserDetailDto();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        [Route("GetAbsenteCount")]
        [HttpGet]
        public async Task<ActionResult<object>> GetAbsenteCount(int sabhaId, int userId)
        {
            try
            {

                var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

                IQueryable<AksharUser> query = await getRoleWiseYuvak(sabhaId, userId);

                //var A1_10 = (from au in query
                //             join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid into auSt
                //             from st in auSt.DefaultIfEmpty()
                //             join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid into stAtt
                //             from att in stAtt.DefaultIfEmpty()
                //             where att == null || att.Ispresent == false
                //             group au by au.UserId into groupedUsers
                //             //where groupedUsers.Count() >= 1 && groupedUsers.Count() <= 10
                //             select groupedUsers.Key).Count();

                //var result = (from au in query
                //              join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid into auSt
                //              from st in auSt.DefaultIfEmpty()
                //              join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid into stAtt
                //              from att in stAtt.DefaultIfEmpty()
                //              where att == null || att.Ispresent == false
                //              group au by au.UserId into groupedUsers
                //              where groupedUsers.Count() >= 1 && groupedUsers.Count() <= 10
                //              select new { aa = groupedUsers.Key, bb = groupedUsers.Count() }).ToList();




                //var A11_20 = await (from au in query
                //                    join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid into auSt
                //                    from st in auSt.DefaultIfEmpty()
                //                    join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid into stAtt
                //                    from att in stAtt.DefaultIfEmpty()
                //                    where att == null
                //                    group au by au.UserId into groupedUsers
                //                    where groupedUsers.Count() >= 11 && groupedUsers.Count() <= 20
                //                    select groupedUsers.Key).CountAsync();

                //var A21_30 = await (from au in query
                //                    join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid into auSt
                //                    from st in auSt.DefaultIfEmpty()
                //                    join att in _context.Attendences on st.Sabhatrackid equals att.Sabhatrackid into stAtt
                //                    from att in stAtt.DefaultIfEmpty()
                //                    where att == null
                //                    group au by au.UserId into groupedUsers
                //                    where groupedUsers.Count() >= 21 && groupedUsers.Count() <= 30
                //                    select groupedUsers.Key).CountAsync();

                var absentData = (from au in query
                                  join st in _context.SabhaTracks on au.Sabhaid equals st.Sabhaid
                                  join att in _context.Attendences on new { x1 = st.Sabhatrackid, x2 = au.UserId } equals new { x1 = att.Sabhatrackid.Value, x2 = att.Userid.Value }
                                  where att.Ispresent == false
                                  group au by au.UserId into g
                                  select new
                                  {
                                      UserId = g.Key,
                                      AbsentCount = g.Count()
                                  }).ToList();

                return new AbsenceCountDto
                {
                    A1_10 = absentData.Where(x => x.AbsentCount >= 1 && x.AbsentCount <= 10).Count(),
                    A11_20 = absentData.Where(x => x.AbsentCount >= 11 && x.AbsentCount <= 20).Count(),
                    A21_30 = absentData.Where(x => x.AbsentCount >= 21 && x.AbsentCount <= 30).Count()
                };
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

    }
}
