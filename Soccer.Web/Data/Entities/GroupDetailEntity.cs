using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Soccer.Web.Data.Entities
{
    public class GroupDetailEntity
    {
        public int Id { get; set; }
        public TeamEntity Team { get; set; }

        [Display(Name = "Matches Played")]
        public int MatchesPlayed { get; set; }

        [Display(Name = "Matches Won")]
        public int MatchesWon { get; set; }

        [Display(Name = "Matches Tied")]
        public int MatchesTied { get; set; }

        [Display(Name = "Matches Lost")]
        public int MatchesLost { get; set; }

        public int Points => MatchesWon * 3 + MatchesTied;

        [Display(Name = "Goals For")]
        public int GoalsFor { get; set; }

        [Display(Name = "Goals Againts")]
        public int GoalsAgaints { get; set; }

        [Display(Name = "Goals Difference")]
        public int GoalsDifference => GoalsFor - GoalsAgaints;

        public GroupEntity Group { get; set; }
    }
}
