using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ICoverterHelper _coverter;

        public PredictionsController(DataContext context, ICoverterHelper coverter)
        {
            _context = context;
            _coverter = coverter;
        }

        [HttpPost]
        [Route("GetPredictionsForUser")]
        public async Task<IActionResult> GetPredictionsForUser([FromBody] PredictionForUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CultureInfo info = new CultureInfo(request.CultureInfo); //nos manda el idioma
            Resource.Culture = info; //enviame el idioma que tiene el telèfono

            TournamentEntity tournament = await _context.Tournaments.FindAsync(request.TournamentId); //busco el torneo
            if (tournament == null)
            {
                return BadRequest(Resource.TournamentDoesntExists); //nos devuelve un literal
            }

            //nos dà las predicciones de un user
            UserEntity userEntity = await _context.Users.Include(u => u.Team)
                                                  .Include(u => u.Predictions)
                                                  .ThenInclude(p => p.Match)
                                                  .ThenInclude(m => m.Local)
                                                  .Include(u => u.Predictions)
                                                  .ThenInclude(p => p.Match)
                                                  .ThenInclude(m => m.Visitor)
                                                  .Include(u => u.Predictions)
                                                  .ThenInclude(p => p.Match)
                                                  .ThenInclude(m => m.Group)
                                                  .ThenInclude(p => p.Tournament)
                                                  .FirstOrDefaultAsync(u => u.Id == request.UserId.ToString()); //de acuerdo al userid que enviaron en el request

            if (userEntity == null)
            {
                return BadRequest(Resource.UserDoesntExists);
            }

            //Add precitions que ya realizé para modificarles
            List<PredictionResponse> predictionResponses = new List<PredictionResponse>();

            foreach (PredictionEntity predictionEntity in userEntity.Predictions) //arma la lista de predicciones
            {
                // si esa predicción es del torneo me la adiciona
                if (predictionEntity.Match.Group.Tournament.Id == request.TournamentId)
                {
                    predictionResponses.Add(_coverter.ToPredictionResponse(predictionEntity));
                }
            }

            // Add precitions que no e hecho
            List<MatchEntity> matches = await _context.Matches.Include(m => m.Local)
                                                                    .Include(m => m.Visitor)
                                                                    .Where(m => m.Group.Tournament.Id == request.TournamentId) //dame los partidos de un torneo
                                                                    .ToListAsync();

            foreach (MatchEntity matchEntity in matches)
            {
                //busca si ya realizé una predicción para ese partido
                PredictionResponse predictionResponse = predictionResponses.FirstOrDefault(pr => pr.Match.Id == matchEntity.Id);
                if (predictionResponse == null) //si no a realizado la predicciòn lo adiciona
                {
                    predictionResponses.Add(new PredictionResponse //me añade una nueva predicción con los datos del partido
                    {
                        Match = _coverter.ToMatchResponse(matchEntity)
                    });
                }
            }

            //devuelve la colección de predicciones, las oredena por el id de predicción y luego por la fecha 
            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.Match.Date));
        }
    }
}
