using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Prism.Views;

namespace Soccer.Prism.ViewModels
{
    public class TournamentItemViewModel : TournametResponse
    {
        private readonly INavigationService _navigation;
        private DelegateCommand _selectTournamentCommand;

        public TournamentItemViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        public DelegateCommand SelectTournamentCommand => _selectTournamentCommand ?? //si no esta establecido
                                                         (_selectTournamentCommand = new DelegateCommand(SelectTournamentAsync));

        private async void SelectTournamentAsync()
        {
            var parameters = new NavigationParameters
            {
                {"tournament", this } //tournament es la llave objeto, this es el valor
            };

            await _navigation.NavigateAsync(nameof(GroupsPage), parameters); //navega a GroupPage pasando un parameters
        }
    }
}
