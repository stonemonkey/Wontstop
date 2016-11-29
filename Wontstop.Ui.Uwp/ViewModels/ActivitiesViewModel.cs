// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Activities;
using RunKeeper.WinRT.HealthGraph.Authorization;
using RunKeeper.WinRT.HealthGraph.User;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ActivitiesViewModel : IHandle<BusyMessage>
    {
        public bool Busy { get; private set; }

        public History History { get; }

        public AuthorizationSession Session { get; }

        private readonly UserResources _userResources;
        private readonly IEventAggregator _eventAggregator;

        public ActivitiesViewModel(
            IEventAggregator eventAggregator, 
            History history,
            UserResources userResources,
            AuthorizationSession authorizationSession)
        {
            _eventAggregator = eventAggregator;
            History = history;
            Session = authorizationSession;
            _userResources = userResources;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));
        
        protected virtual async Task LoadAsync()
        {
            Busy = true;

            try
            {
                await LoadHistoryAsync();
            }
            catch (Exception exception)
            {
                _eventAggregator.PublishOnCurrentThread(exception);
            }
            finally
            {
                Busy = false;
            }
        }

        private async Task LoadHistoryAsync()
        {
            await _userResources.LoadAsync();
            History.SetResource(_userResources.Activities);
            await History.LoadAsync();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(async () => await UnloadAsync()));
        
        protected virtual Task UnloadAsync()
        {
            return Task.FromResult(true);
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
