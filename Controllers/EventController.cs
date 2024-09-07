using Dapper;
using iAkshar.Dto;
using iAkshar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly AskharyatraContext _context;
        private readonly IDbConnection _dbConnection;

        public EventController(AskharyatraContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;   
        }

        //public async Task<ActionResult<object>> GetSabhaListByDate(string inputdateTime)
        //{
        //    try
        //    {
        //        var currentDayOfWeek = Convert.ToDateTime(inputdateTime).DayOfWeek.ToString();
        //        var query = from sabhatrack in _context.SabhaTracks
        //                    join sabha in _context.Sabhas
        //                    on sabhatrack.Sabhaid equals sabha.Sabhaid
        //                    join sabhatype in _context.SabhaTypes
        //                    on sabha.Sabhatypeid equals sabhatype.Sabhatypeid
        //                    join mandal in _context.Mandals
        //                    on sabha.Mandalid equals mandal.Mandalid
        //                    where sabhatrack.Date == Convert.ToDateTime(inputdateTime)
        //                    select new WeeklyEventDto
        //                    {
        //                        SabhaName = sabha.Sabhaname,
        //                        SabhaTime = sabha.Sabhatime,
        //                        MandalName = mandal.Mandalname,
        //                        Area = mandal.Area,
        //                        SabhaId = sabha.Sabhaid,
        //                        SabhaType = sabha.Sabhatype.Sabhatype1
        //                    };
        //        var result = await query.ToListAsync();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        return e.ToString();
        //    }
        //}
        [Route("getsabhalistbydate")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeeklyEventDto>>> GetSabhaListByDate(string inputdateTime)
        {
            try
            {
                // Prepare the input parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@InputDateTime", Convert.ToDateTime(inputdateTime), DbType.DateTime);

                // Name of the stored procedure
                string storedProc = "GetSabhaListByDate";

                // Execute the stored procedure using Dapper
                var result = await _dbConnection.QueryAsync<WeeklyEventDto>(
                    storedProc,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // Return the result as a list
                return Ok(result);
            }
            catch (Exception e)
            {
                // Return the exception details in case of an error
                return BadRequest(e.ToString());
            }
        }


        //[Route("getyuvaklistbysabhaidold/{userId}")]
        //[HttpGet]
        //public ActionResult<List<EventYuvakDto>> GetYuvakListBySabhaIdOld(int userId, int sabhaId)
        //{
        //    try
        //    {
        //        List<EventYuvakDto> result = new List<EventYuvakDto>();
        //        var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

        //        if (user != null && user.Roleid.HasValue)
        //        {
        //            switch ((CommonEnum.Role)user.Roleid.Value)
        //            {
        //                case CommonEnum.Role.Karyakarta:
        //                    var karyakartaQuery = from yuvak in _context.Users
        //                                          join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
        //                                          //join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
        //                                          where sabha.Sabhaid == sabhaId && yuvak.Referenceby == userId
        //                                          select new EventYuvakDto
        //                                          {
        //                                              YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
        //                                              YuvakId = yuvak.UserId,
        //                                              SabhaID = sabha.Sabhaid
        //                                          };
        //                    result.AddRange(karyakartaQuery);
        //                    break;

        //                case CommonEnum.Role.Leader:
        //                    var leaderQuery = from yuvak in _context.Users
        //                                      join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
        //                                      //join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
        //                                      where sabha.Sabhaid == sabhaId
        //                                      select new EventYuvakDto
        //                                      {
        //                                          YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
        //                                          YuvakId = yuvak.UserId,
        //                                          SabhaID = sabha.Sabhaid
        //                                      };
        //                    result.AddRange(leaderQuery);
        //                    break;

        //                case CommonEnum.Role.MandalHead:
        //                    var mandalId = user.Sabha?.Mandalid;
        //                    if (mandalId.HasValue)
        //                    {
        //                        var mandalHeadQuery = from yuvak in _context.Users
        //                                              join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
        //                                              join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
        //                                              where sabha.Mandalid == mandalId.Value
        //                                              select new EventYuvakDto
        //                                              {
        //                                                  YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
        //                                                  YuvakId = yuvak.UserId,
        //                                                  SabhaID = sabha.Sabhaid
        //                                              };
        //                        result.AddRange(mandalHeadQuery);
        //                    }
        //                    break;

        //                case CommonEnum.Role.PradeshHead:
        //                    var city = user.Sabha?.Mandal?.City;
        //                    if (city != null)
        //                    {
        //                        var pradeshHeadQuery = from yuvak in _context.Users
        //                                               join sabha in _context.Sabhas on yuvak.Sabhaid equals sabha.Sabhaid
        //                                               join mandal in _context.Mandals on sabha.Mandalid equals mandal.Mandalid
        //                                               where mandal.City == city
        //                                               select new EventYuvakDto
        //                                               {
        //                                                   YuvakName = yuvak.Firstname + " " + yuvak.Lastname,
        //                                                   YuvakId = yuvak.UserId,
        //                                                   SabhaID = sabha.Sabhaid
        //                                               };
        //                        result.AddRange(pradeshHeadQuery);
        //                    }
        //                    break;
        //            }
        //        }


        //        foreach (var item in result)
        //        {
        //            var lastSabhaRecord = _context.SabhaTracks.Where(x => x.Sabhaid == item.SabhaID).OrderByDescending(x => x.Date).FirstOrDefault();

        //            if (lastSabhaRecord != null)
        //            {
        //                var lastAttendence = _context.Attendences.Where(x => x.Sabhatrackid == lastSabhaRecord.Sabhatrackid && x.Userid == item.YuvakId).FirstOrDefault();

        //                item.LastAttending = lastSabhaRecord?.Date;
        //                item.IsPresent = lastAttendence == null ? false : lastAttendence.Ispresent.Value;
        //            }
        //        }
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        [Route("getyuvaklistbysabhaid/{userId}")]
        [HttpGet]
        public async Task<ActionResult<List<EventYuvakDto>>> GetYuvakListBySabhaId(int userId, int sabhaId)
        {
            try
            {
                // Prepare the parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId, DbType.Int32);
                parameters.Add("@SabhaId", sabhaId, DbType.Int32);

                // Stored procedure name
                string storedProc = "GetYuvakListBySabhaId";

                // Execute the stored procedure using Dapper
                var yuvakList = (await _dbConnection.QueryAsync<EventYuvakDto>(
                    storedProc,
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                return yuvakList;
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }


        //public async Task<ActionResult<YuvakDto>> GetYuvakById(int yuvakId)
        //{
        //    try
        //    {
        //        var query = (from yuvak in _context.Users
        //                     where yuvak.UserId == yuvakId
        //                     select new YuvakDto
        //                     {
        //                         YuvakId = yuvakId,
        //                         Firstname = yuvak.Firstname,
        //                         Lastname = yuvak.Lastname,
        //                         Mobile = yuvak.Mobile,
        //                         Emailid = yuvak.Emailid,
        //                         BirthDate = yuvak.BirthDate,
        //                         Address = yuvak.Address,
        //                         Pincode = yuvak.Pincode,
        //                         Iskaryakarta = yuvak.Iskaryakarta,
        //                         FollowupBy = yuvak.Followupby,
        //                         ReferenceBy = yuvak.Referenceby,
        //                         Gender = yuvak.Gender,
        //                         SabhaId = yuvak.Sabhaid.HasValue ? yuvak.Sabhaid.Value : 0,
        //                     }).FirstOrDefault();

        //        if (query != null)
        //        {
        //            var followupYuvakObject = _context.Users.FirstOrDefault(x => x.UserId == query.FollowupBy);
        //            query.FollowupName = followupYuvakObject?.Firstname + " " + followupYuvakObject?.Lastname;

        //            var referenceByupYuvakObject = _context.Users.FirstOrDefault(x => x.UserId == query.ReferenceBy);
        //            if (referenceByupYuvakObject != null)
        //            {
        //                query.ReferenceName = referenceByupYuvakObject?.Firstname + " " + referenceByupYuvakObject?.Lastname;
        //            }
        //            var attendence = _context.Attendences.Where(x => x.Userid == query.YuvakId).OrderByDescending(x => x.Sabhatrackid).FirstOrDefault();
        //            if (attendence != null)
        //            {
        //                query.LastSabhaAttended = _context.SabhaTracks.Where(x => x.Sabhatrackid == attendence.Sabhatrackid).Select(x => x.Date).FirstOrDefault();
        //            }
        //            var totalAttendences = _context.Attendences.Where(x => x.Userid == query.YuvakId);
        //            if (totalAttendences.Count() > 0)
        //            {
        //                var attended = totalAttendences.Where(x => x.Ispresent == true).Count();
        //                query.Percentage = Math.Round((double)attended / totalAttendences.Count() * 100, 2);
        //            }
        //        }
        //        return query;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        [Route("getyuvakbyid")]
        [HttpGet]
        public async Task<ActionResult<YuvakDto>> GetYuvakById(int yuvakId)
        {
            try
            {
                // Assuming _dbConnection is your existing DbConnection object
                var parameters = new DynamicParameters();
                parameters.Add("@YuvakId", yuvakId);

                var query = await _dbConnection.QueryFirstOrDefaultAsync<YuvakDto>(
                    "GetYuvakDetailsById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return query != null ? Ok(query) : NotFound();
            }
            catch (Exception e)
            {
                // Handle exception
                return StatusCode(500, e.Message);
            }
        }



    }
}
