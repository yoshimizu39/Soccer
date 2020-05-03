using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Prism.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Soccer.Prism.ViewModels
{
    public class SoccerMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private UserResponse _user;

        public SoccerMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            LoadUser();
            LoadMenus();
        }

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }


        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            }
        }

        public ObservableCollection<MenuItemViewMode> Menus { get; set; }

        private void LoadMenus()
        {
            List<Common.Models.Menu> menus = new List<Common.Models.Menu>
            {
                new Common.Models.Menu
                {
                    Icon = "tournament",
                    PageName = "TournamentsPage",
                    Title = Languages.Tournaments
                },

                new Common.Models.Menu
                {
                    Icon = "prediccion",
                    PageName = "MyPredictionsPage",
                    Title = Languages.MyPredictions
                },

                new Common.Models.Menu
                {
                    Icon = "posicion",
                    PageName = "MyPositionsPage",
                    Title = Languages.MyPositions
                },

                new Common.Models.Menu
                {
                    Icon = "user",
                    PageName = "ModifyUserPage",
                    Title = Languages.ModifyUser
                },

                new Common.Models.Menu
                {
                    Icon = "login",
                    PageName = "LoginPage",
                    Title = Settings.IsLogin ? Languages.Logout : Languages.Login
                }
            };

            Menus = new ObservableCollection<MenuItemViewMode>(menus.Select(m => new MenuItemViewMode(_navigationService)
            {
                Icon = m.Icon,
                PageName = m.PageName,
                Title = m.Title,
                //IsLoginRequired = m.IsLoginRequired
            }).ToList());
        } 
    }
}
