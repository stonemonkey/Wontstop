// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using HttpApiClient;
using Mvvm.WinRT;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
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

        public string SelectedGym { get; set; }

        public IDictionary<string, string> Gyms { get; } = new Dictionary<string, string>
        {
            { "Telefonplan", "8" },
            { "Solna", "102" },
            { "Akalla", "103" },
        }; 

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
                      !string.IsNullOrWhiteSpace(Password) &&
                      !string.IsNullOrWhiteSpace(SelectedGym)));

        protected virtual async Task LoginAsync()
        {
            Busy = true;

            (await _requestsFactory.CreateLoginRequest(Gyms[SelectedGym], Email, Password)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleResponse)
                    .OnRequestFailure(x =>
                        _eventAggregator.PublishOnCurrentThread(x.Exception))
                    .OnResponseFailure(x =>
                        _eventAggregator.PublishOnCurrentThread(new ErrorMessage(x.GetContent())));

            Busy = false;
        }

        private void HandleResponse(ProblematorJsonParser parser)
        {
            if (parser.IsError())
            {
                _eventAggregator.PublishOnCurrentThread(
                    new ErrorMessage(parser.GetErrorMessage()));
            }
            else
            {
                var context = parser.To<UserContext>();
                _requestsFactory.SetUserContext(context);
                _storageService.SaveLocal(Settings.ContextKey, context);

                _navigationService.Navigate(typeof (MainPage));
            }
        }
    }
}