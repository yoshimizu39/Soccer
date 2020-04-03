using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface ICoverterHelper
    {
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

    }
}
