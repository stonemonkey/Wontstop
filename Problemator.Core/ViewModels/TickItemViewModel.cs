// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TickItemViewModel : INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        private Tick _tick;
        [Model]
        public Tick Tick
        {
            get { return _tick; }
            set
            {
                _tick = value;
                TryUpdateAscentType();
            }
        }

        public bool Busy { get; private set; }

        public string AscentType { get; private set; }

        private readonly Session _session;
        private readonly Sections _sections;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly ProblematorRequestsFactory _requestFactory;

        public TickItemViewModel(
            Session session,
            Sections sections,
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ProblematorRequestsFactory requestFactory)
        {
            _session = session;
            _sections = sections;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _requestFactory = requestFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            await _session.LoadAsync(false);
            TryUpdateAscentType();
        }

        private void TryUpdateAscentType()
        {
            if (_tick != null && _session.IsLoaded())
            {
                AscentType = _session.GetSportAscentType(_tick.AscentTypeId);
            }
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
            (await _requestFactory.CreateDeleteTickRequest(Tick.Id)
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

            _eventAggregator.PublishOnCurrentThread(Tick);
        }
    }
}