using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Prism.Helpers;

namespace Soccer.Prism.ViewModels
{
    public class PredictionsTabbedPageViewModel : ViewModelBase
    {
        private TournametResponse _tournament;

        public PredictionsTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Languages.PredictionsFor;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("tournament"))
            {
                _tournament = parameters.GetValue<TournametResponse>("tournament");
                Title = $"{Languages.PredictionsFor}: {_tournament.Name}";
            }
        }

    }
}
