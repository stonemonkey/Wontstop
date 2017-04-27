// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using PropertyChanged;
using Wontstop.Climb.Ui.Uwp.Dtos;
using Wontstop.Climb.Ui.Uwp.Utils;
using Wontstop.Climb.Ui.Uwp.Views;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class LoginViewModel
    {
        public bool Busy { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public LoginViewModel(
            IStorageService storageService,
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ProblematorRequestsFactory requestsFactory)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _requestsFactory = requestsFactory;
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

            (await _requestsFactory.CreateLoginRequest(Email, Password)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);

            Busy = false;
        }

        private void HandleResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var context = parser.To<UserContext>();
            _requestsFactory.SetUserContext(context);
            _storageService.SaveLocal(Settings.ContextKey, context);

            _navigationService.Navigate(typeof (MainPage));
        }
    }
}