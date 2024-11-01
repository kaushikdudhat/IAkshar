using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;
using Microsoft.AspNetCore.Http.HttpResults;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MandalController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public MandalController(AskharyatraContext context)
        {
            _context = context;
        }

        // GET: api/Mandal
        [HttpGet]
        public async Task<ActionResult<object>> GetMandals()
        {
            if (_context.Mandals == null)
            {
                return Common.Common.GenerateError("Mandal Not Found");
            }
            var result = await _context.Mandals.ToListAsync();
            return Common.Common.GenerateSuccResponse(result);
        }

        // GET: api/Mandal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mandal>> GetMandal(int id)
        {
            if (_context.Mandals == null)
            {
                return NotFound();
            }
            var mandal = await _context.Mandals.FindAsync(id);

            if (mandal == null)
            {
                return NotFound();
            }

            return mandal;
        }

        // PUT: api/Mandal/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMandal(int id, Mandal mandal)
        {
            if (id != mandal.Mandalid)
            {
                return BadRequest();
            }

            _context.Entry(mandal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MandalExists(id))
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

        // POST: api/Mandal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostMandal(MandalDto mandal)
        {
            if (_context.Mandals == null)
            {
                return Problem("Entity set 'AskharyatraContext.Mandals'  is null.");
            }
            Mandal entity = new Mandal();
            entity.Mandalname = mandal.Mandalname;
            entity.Area = mandal.Area;
            entity.City = mandal.City;
            entity.Country = mandal.Country;
            entity.State = mandal.State;
            entity.Pradesh = _context.Pradeshes.FirstOrDefault(x => x.Pradeshid == mandal.Pradeshid);
            _context.Mandals.Add(entity);
            await _context.SaveChangesAsync();

            return Ok("Mandal Inserted.");
        }

        // DELETE: api/Mandal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMandal(int id)
        {
            if (_context.Mandals == null)
            {
                return NotFound();
            }
            var mandal = await _context.Mandals.FindAsync(id);
            if (mandal == null)
            {
                return NotFound();
            }

            _context.Mandals.Remove(mandal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MandalExists(int id)
        {
            return (_context.Mandals?.Any(e => e.Mandalid == id)).GetValueOrDefault();
        }
    }
}
