// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit.Commands;
using MvvmToolkit.Services;
using Problemator.Core.Models;

namespace Problemator.Core.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy { get; set; }

        private readonly Session _session;
        private readonly UserContext _userContext;
        private readonly INavigationService _navigationService;

        public MainViewModel(
            Session session,
            UserContext userContext,
            INavigationService navigationService)
        {
            _session = session;
            _userContext = userContext;
            _navigationService = navigationService;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            await _session.LoadAsync();
            _navigationService.ClearBackStack();
        }

        private RelayCommand _logoutComand;
        public RelayCommand LogoutCommand => _logoutComand ??
            (_logoutComand = new RelayCommand(async () => await LogoutAsync()));

        protected virtual async Task LogoutAsync()
        {
            Busy = true;

            await _userContext.DeauthenticateAsync();
            _navigationService.Navigate<LoginViewModel>();

            Busy = false;
        }
    }
}