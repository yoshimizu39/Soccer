using Prism.Navigation;
using Soccer.Common.Models;

namespace Soccer.Prism.ViewModels
{
    public class GroupsPageViewModel : ViewModelBase
    {
        private TournametResponse _tournament;
        public GroupsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "GROUPS";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //trae un TournamentResponse de acuerdo a la llave tournament que le asignamos en la clase TournamentItemViewModel
            _tournament = parameters.GetValue<TournametResponse>("tournament");
            Title = _tournament.Name;
        }
    }
}
