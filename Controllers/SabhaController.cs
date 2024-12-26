using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Dapper;
using System.Data;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SabhaController : ControllerBase
    {
        private readonly AskharyatraContext _context;
        private readonly IDbConnection _dbConnection;

        public SabhaController(AskharyatraContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        // GET: api/Sabha
        [HttpGet]
        public async Task<ActionResult<object>> GetSabhas()
        {
            if (_context.Sabhas == null)
            {
                return Common.Common.GenerateError("Sabha Not Found");
            }
            var sabhas = await _context.Sabhas.ToListAsync();
            return Common.Common.GenerateSuccResponse(sabhas);
        }

        // GET: api/Sabha/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sabha>> GetSabha(int id)
        {
            if (_context.Sabhas == null)
            {
                return NotFound();
            }
            var sabha = await _context.Sabhas.FindAsync(id);
            if (sabha == null)
            {
                return NotFound();
            }

            return sabha;
        }

        // PUT: api/Sabha/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSabha(int id, Sabha sabha)
        {
            if (id != sabha.Sabhaid)
            {
                return BadRequest();
            }

            _context.Entry(sabha).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SabhaExists(id))
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

        // POST: api/Sabha
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostSabha(SabhaDto sabha)
        {
            if (_context.Sabhas == null)
            {
                return Problem("Entity set 'AskharyatraContext.Sabhas'  is null.");
            }
            Sabha entity = new Sabha();
            entity.Sabhatime = sabha.Sabhatime;
            entity.Sabhaday = sabha.Sabhaday;
            entity.Sabhaname = sabha.Sabhaname;
            entity.Address = sabha.Address;
            entity.Area = sabha.Area;
            entity.Googlemap = sabha.Googlemap;
            entity.Mandalid = sabha.Mandalid;
            entity.Contactpersonid = sabha.Contactpersonid;
            entity.Sabhatypeid = sabha.Sabhatypeid;
            _context.Sabhas.Add(entity);
            await _context.SaveChangesAsync();

            return Ok("Sabha Inserted.");
        }

        // DELETE: api/Sabha/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSabha(int id)
        {
            if (_context.Sabhas == null)
            {
                return NotFound();
            }
            var sabha = await _context.Sabhas.FindAsync(id);
            if (sabha == null)
            {
                return NotFound();
            }

            _context.Sabhas.Remove(sabha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SabhaExists(int id)
        {
            return (_context.Sabhas?.Any(e => e.Sabhaid == id)).GetValueOrDefault();
        }

        [Route("GetSabhaListByUserId")]
        [HttpGet]
        public async Task<ActionResult<object>> GetSabhaListByUserId(int userid)
        {
            try
            {
                // Assuming _dbConnection is your existing DbConnection object
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userid);

                IEnumerable<SabhaDDDto> query = await _dbConnection.QueryAsync<SabhaDDDto>(
                    "GetSabhaListByUserId",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                 

                return Common.Common.GenerateSuccResponse(query);
            }
            catch (Exception e)
            {
                // Handle exception
                return Common.Common.GenerateError(e.Message);
            }
        }

        [Route("getYuvakCountbyFollowupId/{userId}")]
        [HttpGet]
        public async Task<ActionResult<object>> getYuvakCountbyFollowupId(int userId, int sabhaId)
        {
            try
            {
                // Prepare the parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId, DbType.Int32);
                parameters.Add("@SabhaId", sabhaId, DbType.Int32);
                parameters.Add("@isCount", true, DbType.Boolean);

                // Stored procedure name
                string storedProc = "GetYuvakListBySabhaId";

                // Execute the stored procedure using Dapper
                var count = (await _dbConnection.QueryAsync<YuvakCountDto>(
                    storedProc,
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                return Common.Common.GenerateSuccResponse(count);
            }
            catch (Exception e)
            {
                return Common.Common.GenerateError(e.Message);
            }
        }
    }
}
