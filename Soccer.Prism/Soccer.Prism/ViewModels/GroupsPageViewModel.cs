using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using System.Collections.Generic;

namespace Soccer.Prism.ViewModels
{
    public class GroupsPageViewModel : ViewModelBase
    {
        private readonly ITransformHelper _transform;
        private TournametResponse _tournament;
        private List<Group> _groups;

        public GroupsPageViewModel(INavigationService navigationService, ITransformHelper transform) : base(navigationService)
        {
            _transform = transform;
            Title = "GROUPS";
        }

        public List<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //trae un TournamentResponse de acuerdo a la llave tournament que le asignamos en la clase TournamentItemViewModel
            _tournament = parameters.GetValue<TournametResponse>("tournament");
            Groups = _transform.ToGroups(_tournament.Groups); //en tournamentresponse en el atributo GROUP tiene que devolver un List<>
        }
    }
}
