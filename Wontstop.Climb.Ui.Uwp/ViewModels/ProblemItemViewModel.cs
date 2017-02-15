// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using Mvvm.WinRT;
using Mvvm.WinRT.AttachedProperties;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using Wontstop.Climb.Ui.Uwp.Dtos;
using Wontstop.Climb.Ui.Uwp.Utils;
using Wontstop.Climb.Ui.Uwp.Views;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemItemViewModel
    {
        public bool Busy { get; private set; }
     
        [Model]
        public Problem Problem { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly ProblematorRequestsFactory _requestFactory;

        public ProblemItemViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ProblematorRequestsFactory requestFactory)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _requestFactory = requestFactory;

        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(Load));

        private void Load()
        {
            _eventAggregator.Subscribe(this);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _editTickCommand;
        public RelayCommand EditTickCommand => _editTickCommand ??
            (_editTickCommand = new RelayCommand(EditTick, () => !Busy));

        private void EditTick()
        {
            _navigationService.Navigate(typeof (ProblemDetailesPage), Problem);
        }

        private RelayCommand _deleteTickCommand;
        public RelayCommand DeleteTickCommand => _deleteTickCommand ??
            (_deleteTickCommand = new RelayCommand(
                async () => await DeleteTickAsync(), () => !Busy));

        private async Task DeleteTickAsync()
        {
            Busy = true;

            await RemoveTickAsync();

            Busy = false;
        }

        private async Task RemoveTickAsync()
        {
            (await _requestFactory.CreateDeleteTickRequest(Problem.Tick.Id)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleRemoveResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleRemoveResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            _eventAggregator.PublishOnCurrentThread(Problem);
        }
    }
}