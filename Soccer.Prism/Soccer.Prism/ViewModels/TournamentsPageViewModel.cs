﻿using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class TournamentsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ApiService _apiService;
        private List<TournamentItemViewModel> _tournaments;
        private bool _isRunning;

        public TournamentsPageViewModel(INavigationService navigationService, ApiService apiService) : base(navigationService)
        {
            Title = "Tournaments";
            _navigationService = navigationService;
            _apiService = apiService; //permite cargar los torneos
            LoadTournamentsAsync(); //carga los torneos
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public List<TournamentItemViewModel> Tournaments
        {
            get => _tournaments; //pasa a la lista
            set => SetProperty(ref _tournaments, value); //cuando me cambien el valor de _tournaments, SetProperty refresca la interfaz
        }

        private async void LoadTournamentsAsync()
        {
            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString(); //trae la API
            Response response = await _apiService.GetListAsync<TournametResponse>(url, "/api", "/Tournaments"); //trae la lista

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
            }

            var tournaments = (List<TournametResponse>)response.Result; //trae el obj response como lista de TournamentResponse

            Tournaments = tournaments.Select(t => new TournamentItemViewModel(_navigationService)
            {
                EndDate = t.EndDate,
                Groups = t.Groups,
                Id = t.Id,
                IsActive = t.IsActive,
                LogoPath = t.LogoPath,
                Name = t.Name,
                StartDate = t.StartDate
            }).ToList();
        }
    }
}
