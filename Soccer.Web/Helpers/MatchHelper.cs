using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class MatchHelper : IMatchHelper
    {
        private readonly DataContext _context;
        private MatchEntity _matchEntity;
        private MatchStatus _matchStatus;

        public MatchHelper(DataContext context)
        {
            _context = context;
        }
        public async Task CloseMatchAsync(int matchid, int goalslocal, int goalsvisitor)
        {
            _matchEntity = await _context.Matches.Include(m => m.Local)
                                                 .Include(m => m.Visitor)
                                                 .Include(m => m.Predictions)
                                                 .Include(m => m.Group)
                                                 .ThenInclude(g => g.GroupDetails)
                                                 .ThenInclude(gd => gd.Team)
                                                 .FirstOrDefaultAsync(m => m.Id == matchid);

            _matchEntity.GoalsLocal = goalslocal;
            _matchEntity.GoalsVisitor = goalsvisitor;
            _matchEntity.IsClosed = true;

            //nos dice si ganò el local o el visitante
            _matchStatus = GetMatchStatus(_matchEntity.GoalsLocal.Value, _matchEntity.GoalsVisitor.Value);

            UpdatePointsInPredictions();
            UpdatePositions(); //actualiza las posiciones

            await _context.SaveChangesAsync();
        }

        private void UpdatePositions()
        {
            //busca el equipo local y visitante
            GroupDetailEntity local = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Local);
            GroupDetailEntity visitor = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Visitor);

            //suma 1 en los partidos locales
            local.MatchesPlayed++;
            visitor.MatchesPlayed++;

            local.GoalsFor += _matchEntity.GoalsLocal.Value; //suma los goles del local
            local.GoalsAgaints += _matchEntity.GoalsVisitor.Value; //goles en contra del visitante
            visitor.GoalsFor += _matchEntity.GoalsVisitor.Value; //suma los goles del visitante
            visitor.GoalsAgaints += _matchEntity.GoalsLocal.Value; //goles en contra del local

            if (_matchStatus == MatchStatus.LocalWin) //si ganò el local
            {
                local.MatchesWon++; //suma uno a los partidos ganados de local
                visitor.MatchesLost++; //suma 1 a los partidos perdidos por el visitor
            }
            else if (_matchStatus == MatchStatus.VisitorWin) //si ganò elñ visitor
            {
                visitor.MatchesWon++; //suma 1 a los partidos ganados por el visitor
                local.MatchesLost++; //suma uno a los partidos perdidos de local
            }
            else
            {
                //suma 1 a los partidos con empate
                local.MatchesTied++;
                visitor.MatchesTied++;
            }
        }

        //muestra las predicciones de un partido
        private void UpdatePointsInPredictions()
        {
            //por cada predicciòn
            foreach (PredictionEntity predictionEntity in _matchEntity.Predictions)
            {
                predictionEntity.Points = GetPoints(predictionEntity);
            }
        }

        private int GetPoints(PredictionEntity predictionEntity)
        {
            int points = 0;

            if (predictionEntity.GoalsLocal == _matchEntity.GoalsLocal) //si son iguales
            {
                points += 2; //suma dos puntos
            }

            if (predictionEntity.GoalsVisitor == _matchEntity.GoalsVisitor) //si son iguales
            {
                points += 2; //suma dos puntos
            }

            if (_matchStatus == GetMatchStatus(predictionEntity.GoalsLocal.Value, predictionEntity.GoalsVisitor.Value)) //si es empate
            {
                points++; //suma un punto
            }

            return points; //devuelve los puntos
        }

        private MatchStatus GetMatchStatus(int goalslocal, int goalsvisitor)
        {
            if (goalslocal > goalsvisitor) //si es mayor
            {
                return MatchStatus.LocalWin; //ganò el local
            }

            if (goalsvisitor > goalslocal) //si es mayor
            {
                return MatchStatus.VisitorWin; //ganò el local
            }

            return MatchStatus.Tie; //empate
        }
    }
}
