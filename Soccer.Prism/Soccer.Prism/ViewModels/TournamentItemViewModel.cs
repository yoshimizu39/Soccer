using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Prism.Views;
using System;

namespace Soccer.Prism.ViewModels
{
    public class TournamentItemViewModel : TournametResponse
    {
        private readonly INavigationService _navigation;
        private DelegateCommand _selectTournamentCommand;
        private DelegateCommand _selectTournament2Command;

        public TournamentItemViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        public DelegateCommand SelectTournamentCommand => _selectTournamentCommand ?? //si no esta establecido
                                                         (_selectTournamentCommand = new DelegateCommand(SelectTournamentAsync));

        public DelegateCommand SelectTournament2Command => _selectTournament2Command ?? (_selectTournament2Command = new DelegateCommand(SelectTournamentForPredictionAsync));

        private async void SelectTournamentForPredictionAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                {"tournament", this }
            };

            await _navigation.NavigateAsync(nameof(PredictionsTabbedPage), parameters);
        }

        private async void SelectTournamentAsync()
        {
            var parameters = new NavigationParameters
            {
                {"tournament", this } //tournament es la llave objeto, this es el valor
            };

            await _navigation.NavigateAsync(nameof(TournamentTabbedPage), parameters); //navega a GroupPage pasando un parameters
        }
    }
}
