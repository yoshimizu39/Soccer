using Prism.Navigation;
using Soccer.Common.Models;

namespace Soccer.Prism.ViewModels
{
    public class TournamentTabbedPageViewModel : ViewModelBase
    {
        private TournametResponse _tournament;

        public TournamentTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Title = "SOCCER";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _tournament = parameters.GetValue<TournametResponse>("tournament");
            Title = _tournament.Name;
        }
    }
}
