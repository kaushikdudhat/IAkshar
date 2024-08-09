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
    public class PradeshController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public PradeshController(AskharyatraContext context)
        {
            _context = context;
        }

        // GET: api/Pradesh
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pradesh>>> GetPradeshes()
        {
          if (_context.Pradeshes == null)
          {
              return NotFound();
          }
            return await _context.Pradeshes.ToListAsync();
        }

        // GET: api/Pradesh/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pradesh>> GetPradesh(int id)
        {
          if (_context.Pradeshes == null)
          {
              return NotFound();
          }
            var pradesh = await _context.Pradeshes.FindAsync(id);

            if (pradesh == null)
            {
                return NotFound();
            }

            return pradesh;
        }

        // PUT: api/Pradesh/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPradesh(int id, Pradesh pradesh)
        {
            if (id != pradesh.Pradeshid)
            {
                return BadRequest();
            }

            _context.Entry(pradesh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PradeshExists(id))
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

        // POST: api/Pradesh
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPradesh(PradeshDto pradesh)
        {
          if (_context.Pradeshes == null)
          {
              return Problem("Entity set 'AskharyatraContext.Pradeshes'  is null.");
          }
            _context.Pradeshes.Add(new Pradesh { Pradeshname = pradesh.Pradeshname});
            await _context.SaveChangesAsync();

            return Ok("Pradesh Inserted.");
        }

        // DELETE: api/Pradesh/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePradesh(int id)
        {
            if (_context.Pradeshes == null)
            {
                return NotFound();
            }
            var pradesh = await _context.Pradeshes.FindAsync(id);
            if (pradesh == null)
            {
                return NotFound();
            }

            _context.Pradeshes.Remove(pradesh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PradeshExists(int id)
        {
            return (_context.Pradeshes?.Any(e => e.Pradeshid == id)).GetValueOrDefault();
        }
    }
}
