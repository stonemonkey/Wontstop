// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit.Commands;
using MvvmToolkit.Services;
using Problemator.Core.Models;

namespace Problemator.Core.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        private readonly UserContext _userContext;
        private readonly INavigationService _navigationService;

        public LoginViewModel(
            UserContext userContext,
            INavigationService navigationService)
        {
            _userContext = userContext;
            _navigationService = navigationService;
        }

        private RelayCommand _loginComand;

        public RelayCommand LoginCommand => _loginComand ??
            (_loginComand = new RelayCommand(
                async () => await LoginAsync(), 
                () => !string.IsNullOrWhiteSpace(Email) && 
                      !string.IsNullOrWhiteSpace(Password)));

        protected virtual async Task LoginAsync()
        {
            Busy = true;

            var successful = await _userContext.AuthenticateAsync(Email, Password);
            if (successful)
            {
                _navigationService.Navigate<MainViewModel>();
            }

            Busy = false;
        }
    }
}