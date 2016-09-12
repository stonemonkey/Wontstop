// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Mvvm.WinRT;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Authorization;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class RunKeeperSessionViewModel
    {
        public bool IsInitialized { get; private set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly AuthorizationSession _authorizationSession;

        private const string SessionKey = "runKeeperSession";

        public RunKeeperSessionViewModel(
            IEventAggregator eventAggregator, IAuthorizationProvider authorizationProvider)
        {
            _eventAggregator = eventAggregator;
            _authorizationSession = new AuthorizationSession(SessionKey, authorizationProvider);

            IsInitialized = _authorizationSession.IsInitialized();
        }

        private RelayCommand _connectComand;
        public RelayCommand ConnectCommand => _connectComand ?? 
            (_connectComand = new RelayCommand(async () => await ConnectAsync()));

        protected virtual async Task ConnectAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            try
            {
                await _authorizationSession.InitializeAsync();
                IsInitialized = _authorizationSession.IsInitialized();
            }
            catch (Exception exception)
            {
                _eventAggregator.PublishOnCurrentThread(exception);
            }
            finally
            {
                _eventAggregator.PublishOnCurrentThread(new BusyMessage(false));
            }
        }

        private RelayCommand _disconnectComand;
        public RelayCommand DisconnectCommand => _disconnectComand ??
            (_disconnectComand = new RelayCommand(async () => await DisconnectAsync()));
        protected virtual async Task DisconnectAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            try
            {
                await _authorizationSession.UninitializeAsync();
                IsInitialized = _authorizationSession.IsInitialized();
            }
            catch (Exception exception)
            {
                _eventAggregator.PublishOnCurrentThread(exception);
            }
            finally
            {
                _eventAggregator.PublishOnCurrentThread(new BusyMessage(false));
            }
        }
    }
}