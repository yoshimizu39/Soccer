using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ICoverterHelper _helper;

        public TournamentsController(DataContext context, ICoverterHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTournaments()
        {
            List<TournamentEntity> tournamnts = await _context.Tournaments.Include(t => t.Groups)
                                                                          .ThenInclude(g => g.GroupDetails)
                                                                          .ThenInclude(g => g.Team)
                                                                          .Include(t => t.Groups)
                                                                          .ThenInclude(g => g.Matches)
                                                                          .ThenInclude(g => g.Local)
                                                                          .Include(t => t.Groups)
                                                                          .ThenInclude(g => g.Matches)
                                                                          .ThenInclude(g => g.Visitor)
                                                                          .ToListAsync();

            return Ok(_helper.ToTournametResponse(tournamnts)); //OK, serialize en json
        }
    }
}
