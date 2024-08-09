using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;

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
        public async Task<ActionResult<IEnumerable<SabhaTrack>>> GetSabhaTracks()
        {
          if (_context.SabhaTracks == null)
          {
              return NotFound();
          }
            return await _context.SabhaTracks.ToListAsync();
        }

        // GET: api/SabhaTrack/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SabhaTrack>> GetSabhaTrack(int id)
        {
          if (_context.SabhaTracks == null)
          {
              return NotFound();
          }
            var sabhaTrack = await _context.SabhaTracks.FindAsync(id);

            if (sabhaTrack == null)
            {
                return NotFound();
            }

            return sabhaTrack;
        }

        // PUT: api/SabhaTrack/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSabhaTrack(int id, SabhaTrack sabhaTrack)
        {
            if (id != sabhaTrack.Sabhatrackid)
            {
                return BadRequest();
            }

            _context.Entry(sabhaTrack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SabhaTrackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SabhaTrack
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostSabhaTrack(SabhaTrackDto sabhaTrack)
        {
          if (_context.SabhaTracks == null)
          {
              return Problem("Entity set 'AskharyatraContext.SabhaTracks'  is null.");
          }

            SabhaTrack entity = new SabhaTrack();
            entity.Sabhaid = sabhaTrack.Sabhaid;
            entity.Topic= sabhaTrack.Topic; 
            entity.Date= sabhaTrack.Date;
            _context.SabhaTracks.Add(entity);
            await _context.SaveChangesAsync();

            var users = _context.Users.Where(x => x.Sabhaid == sabhaTrack.Sabhaid).ToList();
            foreach (var user in users)
            {
                Attendence attendence = new Attendence();
                attendence.Sabhatrackid = entity.Sabhatrackid;
                attendence.Userid= user.UserId;
                attendence.Ispresent = false;
                _context.Attendences.Add(attendence);
            }
            await _context.SaveChangesAsync();
            return Ok("SabhaTrack Inserted.");
        }

        // DELETE: api/SabhaTrack/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSabhaTrack(int id)
        {
            if (_context.SabhaTracks == null)
            {
                return NotFound();
            }
            var sabhaTrack = await _context.SabhaTracks.FindAsync(id);
            if (sabhaTrack == null)
            {
                return NotFound();
            }

            _context.SabhaTracks.Remove(sabhaTrack);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SabhaTrackExists(int id)
        {
            return (_context.SabhaTracks?.Any(e => e.Sabhatrackid == id)).GetValueOrDefault();
        }
    }
}
