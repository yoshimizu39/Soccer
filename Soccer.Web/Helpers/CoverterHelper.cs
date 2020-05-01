using Microsoft.EntityFrameworkCore;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class CoverterHelper : ICoverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combo;

        public CoverterHelper(DataContext context, ICombosHelper combo)
        {
            _context = context;
            _combo = combo;
        }
        public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
        {
            return new TeamEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name
            };
        }

        public TeamViewModel ToTeamViewModel(TeamEntity teamEntity)
        {
            return new TeamViewModel
            {
                Id = teamEntity.Id,
                LogoPath = teamEntity.LogoPath,
                Name = teamEntity.Name
            };
        }

        public TournamentEntity ToTournamentEntity(TournametViewModel model, string path, bool isNew)
        {
            return new TournamentEntity
            {
                EndDate = model.EndDate.ToUniversalTime(),
                Groups = model.Groups,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                LogoPath = path,
                Name = model.Name,
                StartDate = model.StartDate.ToUniversalTime()
            };
        }

        public TournametViewModel ToTournamentViewModel(TournamentEntity tournament)
        {
            return new TournametViewModel
            {
                EndDate = tournament.EndDate,
                Groups = tournament.Groups,
                Id = tournament.Id,
                IsActive = tournament.IsActive,
                LogoPath = tournament.LogoPath,
                Name = tournament.Name,
                StartDate = tournament.StartDate
            };
        }

        public async Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew)
        {
            return new GroupEntity
            {
                GroupDetails = model.GroupDetails,
                Id = isNew ? 0 : model.Id,
                Matches = model.Matches,
                Name = model.Name,
                Tournament = await _context.Tournaments.FindAsync(model.TournamentId)
            };
        }

        public GroupViewModel ToGroupViewModel(GroupEntity groupEntity)
        {
            return new GroupViewModel
            {
                GroupDetails = groupEntity.GroupDetails,
                Id = groupEntity.Id,
                Matches = groupEntity.Matches,
                Name = groupEntity.Name,
                Tournament = groupEntity.Tournament,
                TournamentId = groupEntity.Tournament.Id
            };
        }

        public async Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew)
        {
            return new GroupDetailEntity
            {
                GoalsAgaints = model.GoalsAgaints,
                GoalsFor = model.GoalsFor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                MatchesLost = model.MatchesLost,
                MatchesPlayed = model.MatchesPlayed,
                MatchesTied = model.MatchesTied,
                MatchesWon = model.MatchesWon,
                Team = await _context.Teams.FindAsync(model.TeamId)
            };
        }

        public GroupDetailViewModel ToGroupDetailViewModelAsync(GroupDetailEntity entity)
        {
            return new GroupDetailViewModel
            {
                GoalsAgaints = entity.GoalsAgaints,
                GoalsFor = entity.GoalsFor,
                Group = entity.Group,
                GroupId = entity.Group.Id,
                Id = entity.Id,
                MatchesLost = entity.MatchesLost,
                MatchesPlayed = entity.MatchesPlayed,
                MatchesTied = entity.MatchesTied,
                MatchesWon = entity.MatchesWon,
                Team = entity.Team,
                TeamId = entity.Team.Id,
                Teams = _combo.GetComboTeams()
            };
        }

        public async Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew)
        {
            return new MatchEntity
            {
                Date = model.Date.ToUniversalTime(),
                GoalsLocal = model.GoalsLocal,
                GoalsVisitor = model.GoalsVisitor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                IsClosed = model.IsClosed,
                Local = await _context.Teams.FindAsync(model.LocalId),
                Visitor = await _context.Teams.FindAsync(model.VisitorlId)
            };
        }

        public MatchViewModel ToMatchViewModelAsync(MatchEntity entity)
        {
            return new MatchViewModel
            {
                Date = entity.Date.ToLocalTime(),
                GoalsLocal = entity.GoalsLocal,
                GoalsVisitor = entity.GoalsVisitor,
                Group = entity.Group,
                GroupId = entity.Group.Id,
                Id = entity.Id,
                IsClosed = entity.IsClosed,
                Local = entity.Local,
                Visitor = entity.Visitor,
                LocalId = entity.Local.Id,
                VisitorlId = entity.Visitor.Id,
                Teams = _combo.GetComboTeams(entity.Group.Id)
            };
        }

        public TournametResponse ToTournametResponse(TournamentEntity entity)
        {
            return new TournametResponse
            {
                EndDate = entity.EndDate,
                Id = entity.Id,
                IsActive = entity.IsActive,
                LogoPath = entity.LogoPath,
                Name = entity.Name,
                StartDate = entity.StartDate,
                Groups = entity.Groups?.Select(g => new GroupResponse //por cada grupo me crea un groupresponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    GroupDetails = g.GroupDetails?.Select(gd => new GroupDetailResponse
                    {
                        GoalsAgaints = gd.GoalsAgaints,
                        GoalsFor = gd.GoalsFor,
                        Id = g.Id,
                        MatchesLost = gd.MatchesLost,
                        MatchesPlayed = gd.MatchesPlayed,
                        MatchesTied = gd.MatchesTied,
                        MatchesWon = gd.MatchesWon,
                        Team = ToTeamResponse(gd.Team)
                    }).ToList(),
                    Matches = g.Matches?.Select(m => new MatchResponse
                    {
                        Date = m.Date,
                        GoalsLocal = m.GoalsLocal,
                        GoalsVisitor = m.GoalsVisitor,
                        Id = m.Id,
                        IsClosed = m.IsClosed,
                        Local = ToTeamResponse(m.Local),
                        Visitor = ToTeamResponse(m.Visitor),
                        Predictions = m.Predictions?.Select(p => new PredictionResponse
                        {
                            GoalsLocal = p.GoalsLocal,
                            GoalsVisitor = p.GoalsVisitor,
                            Id = p.Id,
                            Points = p.Points,
                            User = ToUserResponse(p.User)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        public UserResponse ToUserResponse(UserEntity user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                Address = user.Address,
                Document = user.Document,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PicturePath = user.PicturePath,
                Team = ToTeamResponse(user?.Team),
                UserType = user.UserType
            };
        }

        private TeamResponse ToTeamResponse(TeamEntity team)
        {
            if (team == null)
            {
                return null;
            }

            return new TeamResponse
            {
                Id = team.Id,
                LogoPath = team.LogoPath,
                Name = team.Name
            };
        }

        public List<TournametResponse> ToTournametResponse(List<TournamentEntity> entity)
        {
            List<TournametResponse> list = new List<TournametResponse>();
            foreach (TournamentEntity tournament in entity) //por cada torneo que hay en entity
            {
                list.Add(ToTournametResponse(tournament)); //adicioname un tournasmentresponse
            }

            return list;
        }

        public PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity)
        {
            return new PredictionResponse
            {
                GoalsLocal = predictionEntity.GoalsLocal,
                GoalsVisitor = predictionEntity.GoalsVisitor,
                Id = predictionEntity.Id,
                Match = ToMatchResponse(predictionEntity.Match),
                Points = predictionEntity.Points
            };
        }

        public MatchResponse ToMatchResponse(MatchEntity matchEntity)
        {
            return new MatchResponse
            {
                Date = matchEntity.Date,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = ToTeamResponse(matchEntity.Local),
                Visitor = ToTeamResponse(matchEntity.Visitor)
            };
        }

    }
}
