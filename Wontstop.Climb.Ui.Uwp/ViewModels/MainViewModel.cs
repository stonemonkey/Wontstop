// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using HttpApiClient.Parsers;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Services;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    public class MainViewModel
    {
        public bool Busy { get; set; }

        private readonly IStorageService _storageService;
        private readonly INavigationService _navigationService;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public MainViewModel(
            IStorageService storageService,
            INavigationService navigationService,
            ProblematorRequestsFactory requestsFactory)
        {
            _storageService = storageService;
            _navigationService = navigationService;
            _requestsFactory = requestsFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(Load));

        private void Load()
        {
            _navigationService.ClearBackStack();
        }

        private RelayCommand _logoutComand;
        public RelayCommand LogoutCommand => _logoutComand ??
            (_logoutComand = new RelayCommand(async () => await LogoutAsync()));

        protected virtual async Task LogoutAsync()
        {
            Busy = true;

            await _requestsFactory.CreateLogoutRequest()
                .RunAsync<StringParser>();

            _requestsFactory.ClearUserContext();

            _storageService.DeleteLocal(Settings.ContextKey);

            _navigationService.Navigate("Wontstop.Climb.Ui.Uwp.Views.LoginPage, Wontstop.Climb.Ui.Uwp");

            Busy = false;
        }
    }
}