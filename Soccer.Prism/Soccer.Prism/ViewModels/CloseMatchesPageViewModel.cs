﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class CloseMatchesPageViewModel : ViewModelBase
    {
        private TournametResponse _tournamet;
        private List<MatchResponse> _matches;

        public CloseMatchesPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "CLOSED";
        }

        public List<MatchResponse> Matches
        {
            get => _matches;
            set => SetProperty(ref _matches, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _tournamet = parameters.GetValue<TournametResponse>("tournament");
            Title = _tournamet.Name;
            LoadMatches();
        }

        private void LoadMatches()
        {
            List<MatchResponse> matches = new List<MatchResponse>(); //obtiene la colecciòn de matches

            foreach (GroupResponse groups in _tournamet.Groups) //por cada grupo
            {
                matches.AddRange(groups.Matches); //adiciona la colecciòn de matches de cada grupo
            }

            Matches = matches.Where(m => !m.IsClosed).OrderBy(m => m.Date).ToList(); //ordena por fecha de partido
        }
    }
}
