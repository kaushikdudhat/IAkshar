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
    public class SabhaTypeController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public SabhaTypeController(AskharyatraContext context)
        {
            _context = context;
        }

        // GET: api/SabhaType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SabhaType>>> GetSabhaTypes()
        {
          if (_context.SabhaTypes == null)
          {
              return NotFound();
          }
            return await _context.SabhaTypes.ToListAsync();
        }

        // GET: api/SabhaType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SabhaType>> GetSabhaType(int id)
        {
          if (_context.SabhaTypes == null)
          {
              return NotFound();
          }
            var sabhaType = await _context.SabhaTypes.FindAsync(id);

            if (sabhaType == null)
            {
                return NotFound();
            }

            return sabhaType;
        }

        // PUT: api/SabhaType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSabhaType(int id, SabhaType sabhaType)
        {
            if (id != sabhaType.Sabhatypeid)
            {
                return BadRequest();
            }

            _context.Entry(sabhaType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SabhaTypeExists(id))
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

        // POST: api/SabhaType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostSabhaType(SabhaTypeDto sabhaType)
        {
          if (_context.SabhaTypes == null)
          {
              return Problem("Entity set 'AskharyatraContext.SabhaTypes'  is null.");
          }
            SabhaType entity = new SabhaType();
            entity.Sabhatype1 = sabhaType.SabhaType;
            _context.SabhaTypes.Add(entity);
            await _context.SaveChangesAsync();

            return Ok("SabhaType Inserted.");
        }

        // DELETE: api/SabhaType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSabhaType(int id)
        {
            if (_context.SabhaTypes == null)
            {
                return NotFound();
            }
            var sabhaType = await _context.SabhaTypes.FindAsync(id);
            if (sabhaType == null)
            {
                return NotFound();
            }

            _context.SabhaTypes.Remove(sabhaType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SabhaTypeExists(int id)
        {
            return (_context.SabhaTypes?.Any(e => e.Sabhatypeid == id)).GetValueOrDefault();
        }
    }
}
