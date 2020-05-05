using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism.Helpers;
using Soccer.Prism.Views;
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
        private readonly IApiService _apiService;
        private static SoccerMasterDetailPageViewModel _instance; //singelton
        private UserResponse _user;
        private DelegateCommand _modifyUserCommand;

        public SoccerMasterDetailPageViewModel(INavigationService navigationService,
                                               IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            LoadUser();
            LoadMenus();
        }

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public DelegateCommand ModifyUserCommand => _modifyUserCommand ?? (_modifyUserCommand = new DelegateCommand(ModifyUserAsync));

        public static SoccerMasterDetailPageViewModel GetInstance()
        {
            return _instance; //nos devuelve la misma clase
        }

        public async void ReloadUser()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                return;
            }

            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            EmailRequest emailRequest = new EmailRequest
            {
                CultureInfo = Languages.Culture,
                Email = user.Email
            };

            Response response = await _apiService.GetUserByEmail(url, "api", "/Account/GetUserByEmail", "bearer", token.Token, emailRequest);
            UserResponse userResponse = (UserResponse)response.Result;
            Settings.User = JsonConvert.SerializeObject(userResponse); //actualize al user en persistencia
            LoadUser(); //si el user esta logueado lo muestra en persistencia
        }


        private async void ModifyUserAsync()
        {
            await _navigationService.NavigateAsync($"/SoccerMasterDetailPage/NavigationPage/{nameof(ModifyUserPage)}");
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
