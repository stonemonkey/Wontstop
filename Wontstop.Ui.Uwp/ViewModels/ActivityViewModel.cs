// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Authorization;
using RunKeeper.WinRT.HealthGraph.User;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ActivityViewModel : IHandle<BusyMessage>
    {
        public bool Busy { get; private set; }

        public AuthorizationSession Session { get; }

        private readonly UserResources _userResources;

        private readonly IEventAggregator _eventAggregator;

        public ActivityViewModel(
            IEventAggregator eventAggregator,
            UserResources userResources,
            AuthorizationSession authorizationSession)
        {
            _eventAggregator = eventAggregator;

            _userResources = userResources;
            Session = authorizationSession;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));
        
        protected virtual async Task LoadAsync()
        {
            Busy = true;

            try
            {
                await LoadActivityAsync();
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

        private async Task LoadActivityAsync()
        {
            await _userResources.LoadAsync();
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
