﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Utils;
using PropertyChanged;

namespace Problemator.Core.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ProblemItemViewModel
    {
        public bool Busy { get; private set; }
     
        public bool HasTick { get; private set; }

        private Problem _problem;

        [Model]
        public Problem Problem
        {
            get { return _problem; }
            set
            {
                _problem = value;
                UpdateHasTick();
            }
        }

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

        private RelayCommand _addTickCommand;
        public RelayCommand AddTickCommand => _addTickCommand ??
            (_addTickCommand = new RelayCommand(AddTick, () => !Busy));

        private void AddTick()
        {
            Busy = true;

            // TODO: implement
            UpdateHasTick();

            Busy = false;
        }

        private RelayCommand _deleteTickCommand;
        public RelayCommand DeleteTickCommand => _deleteTickCommand ??
            (_deleteTickCommand = new RelayCommand(
                async () => await DeleteTickAsync(), () => !Busy));

        private async Task DeleteTickAsync()
        {
            Busy = true;

            await RemoveTickAsync();
            UpdateHasTick();

            Busy = false;
        }

        private void UpdateHasTick()
        {
            HasTick = _problem?.Tick != null;
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

            _eventAggregator.PublishOnCurrentThread(Problem.Tick);
        }

        private RelayCommand _openDetailsComand;

        public RelayCommand OpenDetailsCommand => _openDetailsComand ??
            (_openDetailsComand = new RelayCommand(OpenDetails, () => !Busy));

        private void OpenDetails()
        {
            _navigationService.Navigate(
                "Wontstop.Climb.Ui.Uwp.Views.ProblemDetailesPage, Wontstop.Climb.Ui.Uwp", Problem);
        }
    }
}