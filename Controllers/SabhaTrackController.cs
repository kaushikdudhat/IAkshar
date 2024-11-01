
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SabhaTrackController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public SabhaTrackController(AskharyatraContext context)
        {
            _context = context;
        }

        // GET: api/SabhaTrack
        [HttpGet]
        public async Task<ActionResult<object>> GetSabhaTracks()
        {
            if (_context.SabhaTracks == null)
            {
                return Common.Common.GenerateError();
            }
            var result = await _context.SabhaTracks.ToListAsync();
            return Common.Common.GenerateSuccResponse(result);
        }

        // GET: api/SabhaTrack/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSabhaTrack(int id)
        {
            if (_context.SabhaTracks == null)
            {
                return Common.Common.GenerateError();
            }
            var sabhaTrack = await _context.SabhaTracks.FindAsync(id);

            if (sabhaTrack == null)
            {
                return Common.Common.GenerateError();
            }

            return Common.Common.GenerateSuccResponse(sabhaTrack);
        }

        // PUT: api/SabhaTrack/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<object> PutSabhaTrack(int id, SabhaTrack sabhaTrack)
        {
            _context.Entry(sabhaTrack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Common.Common.GenerateError(ex.Message);
            }

            return Common.Common.GenerateSuccResponse(null, "Updated Successfully");
        }

        // POST: api/SabhaTrack
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<object> PostSabhaTrack(SabhaTrackDto sabhaTrack)
        {
           

            SabhaTrack entity = new SabhaTrack();
            entity.Sabhaid = sabhaTrack.Sabhaid;
            entity.Topic = sabhaTrack.Topic;
            entity.Date = sabhaTrack.Date;
            _context.SabhaTracks.Add(entity);
            await _context.SaveChangesAsync();

            var users = _context.Users.Where(x => x.Sabhaid == sabhaTrack.Sabhaid).ToList();
            foreach (var user in users)
            {
                Attendence attendence = new Attendence();
                attendence.Sabhatrackid = entity.Sabhatrackid;
                attendence.Userid = user.UserId;
                attendence.Ispresent = false;
                _context.Attendences.Add(attendence);
            }
            await _context.SaveChangesAsync();
            return Common.Common.GenerateSuccResponse(null, "SabhaTrack Inserted");
        }

        // DELETE: api/SabhaTrack/5
        [HttpDelete("{id}")]
        public async Task<object> DeleteSabhaTrack(int id)
        {
            
            var sabhaTrack = await _context.SabhaTracks.FindAsync(id);
            if (sabhaTrack == null)
            {
                return Common.Common.GenerateError("Sabha Track not found");
            }

            _context.SabhaTracks.Remove(sabhaTrack);
            await _context.SaveChangesAsync();

            return Common.Common.GenerateSuccResponse(null, "SabhaTrack deleted");
        }

        private bool SabhaTrackExists(int id)
        {
            return (_context.SabhaTracks?.Any(e => e.Sabhatrackid == id)).GetValueOrDefault();
        }
    }
}
