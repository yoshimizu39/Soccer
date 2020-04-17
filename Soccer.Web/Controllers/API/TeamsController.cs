using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public IEnumerable<TeamEntity> GetTeams()
        {
            return _context.Teams.OrderBy(t => t.Name);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamEntity([FromRoute] int id) //FromRoute, toma el id de la ruta de la URL
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TeamEntity teamEntity = await _context.Teams.FindAsync(id);

            if (teamEntity == null)
            {
                return NotFound();
            }

            return Ok(teamEntity); //Ok, devuelve formato json
        }

        //// PUT: api/Teams/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTeamEntity([FromRoute] int id, [FromBody] TeamEntity teamEntity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != teamEntity.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(teamEntity).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TeamEntityExists(id))
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

        //// POST: api/Teams
        //[HttpPost]
        //public async Task<IActionResult> PostTeamEntity([FromBody] TeamEntity teamEntity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Teams.Add(teamEntity);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTeamEntity", new { id = teamEntity.Id }, teamEntity);
        //}

        //// DELETE: api/Teams/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTeamEntity([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var teamEntity = await _context.Teams.FindAsync(id);
        //    if (teamEntity == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Teams.Remove(teamEntity);
        //    await _context.SaveChangesAsync();

        //    return Ok(teamEntity);
        ////}

        //private bool TeamEntityExists(int id)
        //{
        //    return _context.Teams.Any(e => e.Id == id);
        ////}
    }
}