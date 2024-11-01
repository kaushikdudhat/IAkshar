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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionController : ControllerBase
    {
        private readonly AskharyatraContext _context;

        public SuggestionController(AskharyatraContext context)
        {
            _context = context;
        }

        //// GET: api/Sabha
        //[HttpGet]
        //public async Task<ActionResult<object>> GetSabhas()
        //{
        //    if (_context.Sabhas == null)
        //    {
        //        return Common.Common.GenerateError("Sabha Not Found");
        //    }
        //    var sabhas = await _context.Sabhas.ToListAsync();
        //    return Common.Common.GenerateSuccResponse(sabhas);
        //}

        //// GET: api/Sabha/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Sabha>> GetSabha(int id)
        //{
        //    if (_context.Sabhas == null)
        //    {
        //        return NotFound();
        //    }
        //    var sabha = await _context.Sabhas.FindAsync(id);
        //    if (sabha == null)
        //    {
        //        return NotFound();
        //    }

        //    return sabha;
        //}

        //// PUT: api/Sabha/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutSabha(int id, Sabha sabha)
        //{
        //    if (id != sabha.Sabhaid)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(sabha).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SabhaExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPost]
        public async Task<object> PostSuggession(string note, int userId)
        {
            try
            {
                Suggestions entity = new Suggestions();
                entity.Note = note;
                entity.CreatedOn = DateTime.Now;
                entity.CreatedBy = userId;
                _context.Suggestions.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Common.Common.GenerateError(ex.Message);

            }
            return Common.Common.GenerateSuccResponse(null, "Suggestion Added");
        }

        //// DELETE: api/Sabha/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSabha(int id)
        //{
        //    if (_context.Sabhas == null)
        //    {
        //        return NotFound();
        //    }
        //    var sabha = await _context.Sabhas.FindAsync(id);
        //    if (sabha == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Sabhas.Remove(sabha);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool SabhaExists(int id)
        //{
        //    return (_context.Sabhas?.Any(e => e.Sabhaid == id)).GetValueOrDefault();
        //}
    }
}
