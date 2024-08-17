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
        public async Task<List<BloodCount>> BloodGroupCount()
        {
            return await _context.Users.GroupBy(x => x.Bloodgroup).Where(x => x.Key != null).Select(x =>
             new BloodCount
             {
                 Blood = x.Key,
                 Count = x.Count()
             }).ToListAsync();
        }

        [HttpGet("BloodGroupList")]
        public async Task<List<UserDataDto>> BloodGroupCount(string bloodGroup)
        {

           
                return await (from yuvak in _context.Users
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
                                           YuvakType = Common.Common.GetYuvakType(yuvak)
                                       }).ToListAsync();
             


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
    }

}
