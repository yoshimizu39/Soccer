using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Prism.ViewModels
{
    public class MenuItemViewMode : Menu
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectMenuCommand;

        public MenuItemViewMode(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SselectMenuCommand => _selectMenuCommand ?? (_selectMenuCommand = new DelegateCommand(SelectMenuAsync));

        private async void SelectMenuAsync()
        {
            await _navigationService.NavigateAsync($"/SoccerMasterDetailPage/NavigationPage/{PageName}");
        }
    }
}
