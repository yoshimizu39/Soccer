using Newtonsoft.Json;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class MatchsPageViewModel : ViewModelBase
    {
        private TournametResponse _tournamet;
        private List<MatchResponse> _matches;

        public MatchsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "OPEN";
            LoadMatches();
        }

        public List<MatchResponse> Matches
        {
            get => _matches;
            set => SetProperty(ref _matches, value);
        }

        private void LoadMatches()
        {
            //deserializa el <TournametResponse> de acuerdo a lo que trae el Settings.Tournament
            //de esa manera pasamos el torneo o datos a las otras pestañas
            _tournamet = JsonConvert.DeserializeObject<TournametResponse>(Settings.Tournament);
            //Title = _tournamet.Name;
            List<MatchResponse> matches = new List<MatchResponse>(); //obtiene la colecciòn de matches

            foreach (GroupResponse groups in _tournamet.Groups) //por cada grupo
            {
                matches.AddRange(groups.Matches); //adiciona la colecciòn de matches de cada grupo
            }

            Matches = matches.Where(m => !m.IsClosed).OrderBy(m => m.Date).ToList(); //ordena por fecha de partido
        }
    }
}
