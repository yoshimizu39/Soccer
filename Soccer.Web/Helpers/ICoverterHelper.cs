﻿using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface ICoverterHelper
    {
        PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity);
        MatchResponse ToMatchResponse(MatchEntity matchEntity);
        TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew);
        TeamViewModel ToTeamViewModel(TeamEntity teamEntity);
        TournamentEntity ToTournamentEntity(TournametViewModel model, string path, bool isNew);
        TournametViewModel ToTournamentViewModel(TournamentEntity tournament);
        Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew);
        GroupViewModel ToGroupViewModel(GroupEntity groupEntity);
        Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew);
        GroupDetailViewModel ToGroupDetailViewModelAsync(GroupDetailEntity entity);
        Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew);
        MatchViewModel ToMatchViewModelAsync(MatchEntity entity);
        TournametResponse ToTournametResponse(TournamentEntity entity);
        List<TournametResponse> ToTournametResponse(List<TournamentEntity> entity);
        UserResponse ToUserResponse(UserEntity userEntity);
    }
}
