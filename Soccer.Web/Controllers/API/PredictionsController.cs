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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //tipo de token de seguridad
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ICoverterHelper _coverter;
        private readonly IUserHelper _userHelper;

        public PredictionsController(DataContext context,
                                     ICoverterHelper coverter,
                                     IUserHelper userHelper)
        {
            _context = context;
            _coverter = coverter;
            _userHelper = userHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //traemos los Users, Team, Predictions
            List<UserEntity> users = await _context.Users.Include(u => u.Team)
                                                         .Include(u => u.Predictions)
                                                         .ToListAsync();

            //por cada user creamos una postionresponse
            List<PositionResponse> positionResponses = users.Select(u => new PositionResponse
            {
                Points = u.Points,
                UserResponse = _coverter.ToUserResponse(u)
            }).ToList();

            //creamos una lista de posiciones y la ordenamos descendentemente por los puntos
            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (var item in list)
            {
                item.Ranking = i;
                i++;
            }

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionsByTournament([FromRoute] int id) //id del torneo
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TournamentEntity tournament = await _context.Tournaments.Include(t => t.Groups)                                                                    .Include(t => t.Groups)
                                                                    .ThenInclude(g => g.Matches)
                                                                    .ThenInclude(m => m.Predictions)
                                                                    .ThenInclude(p => p.User)
                                                                    .ThenInclude(u => u.Team)
                                                                    .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return BadRequest("Tournament doesn't exists.");
            }

            //creamos la lista de predicciones
            List<PositionResponse> positionsResponses = new List<PositionResponse>();
            foreach (GroupEntity group in tournament.Groups) //por cada grupo en el torneo
            {
                foreach (MatchEntity match in group.Matches) //por cda partido del grupo
                {
                    foreach (PredictionEntity prediction in match.Predictions) //por cada predicciòn del partido
                    {
                        //buscamos si el user realizò una predicciòn del partido
                        PositionResponse positionResponse = positionsResponses.FirstOrDefault(p => p.UserResponse.Id == prediction.User.Id);
                        if (positionResponse == null)
                        {
                            positionsResponses.Add(new PositionResponse //adicionamos a la lista
                            {
                                Points = prediction.Points, //suma los puntos obtenidos
                                UserResponse = _coverter.ToUserResponse(prediction.User)
                            });
                        }
                        else
                        {
                            positionResponse.Points += prediction.Points; //suma los puntos si el user no es nuevo
                        }
                    }
                }
            }

            //ordenamos descendentemente por los puntos
            List<PositionResponse> list = positionsResponses.OrderByDescending(p => p.Points).ToList();
            int i = 1;
            foreach (PositionResponse item in list) //a cada item
            {
                item.Ranking = i; //le asignamos el puesto
                i++;
            }

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> PostPrediction([FromBody] PredictionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            MatchEntity matchEntity = await _context.Matches.FindAsync(request.MatchId);
            if (matchEntity == null)
            {
                return BadRequest(Resource.MatchDoesntExists);
            }

            if (matchEntity.IsClosed)
            {
                return BadRequest(Resource.MatchAlreadyClosed);
            }

            UserEntity userEntity = await _userHelper.GetUserAsync(request.UserId);
            if (userEntity == null)
            {
                return BadRequest(Resource.UserDoesntExists);
            }

            //si el partido ya empezò
            if (matchEntity.Date <= DateTime.UtcNow)
            {
                return BadRequest(Resource.MatchAlreadyStarts); //no deja realizar predicciones
            }

            //buscamos la predicciòn de ese user y el partido
            PredictionEntity predictionEntity = await _context.Predictions
                                                      .FirstOrDefaultAsync(p => p.User.Id == request.UserId.ToString() &&
                                                      p.Match.Id == request.MatchId);

            if (predictionEntity == null)
            {
                predictionEntity = new PredictionEntity //predicciòn nueva
                {
                    GoalsLocal = request.GoalsLocal,
                    GoalsVisitor = request.GoalsVisitor,
                    Match = matchEntity,
                    User = userEntity
                };

                _context.Predictions.Add(predictionEntity);
            }
            else
            {
                predictionEntity.GoalsLocal = request.GoalsLocal;
                predictionEntity.GoalsVisitor = request.GoalsVisitor;
                _context.Predictions.Update(predictionEntity);
            }

            await _context.SaveChangesAsync();

            return NoContent();
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
