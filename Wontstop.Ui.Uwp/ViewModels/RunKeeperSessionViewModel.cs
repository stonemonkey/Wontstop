// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph;
using RunKeeper.WinRT.HealthGraph.Authorization;
using RunKeeper.WinRT.HealthGraph.User;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class RunKeeperSessionViewModel
    {
        /// <summary>
        /// Specifies if current RunKeeper session is authorized.
        /// </summary>
        public bool IsAuthorized { get; private set; }

        /// <summary>
        /// User profile data
        /// </summary>
        public UserProfile Profile { get; private set; }

        private UserResources _resources;
        private readonly AuthorizationSession _session;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="authorizationProvider"></param>
        public RunKeeperSessionViewModel(
            IEventAggregator eventAggregator, IAuthorizationProvider authorizationProvider)
        {
            _eventAggregator = eventAggregator;
            _session = new AuthorizationSession(authorizationProvider);

            IsAuthorized = _session.IsAuthorized();
        }

        private RelayCommand _loadComand;
        /// <summary>
        /// Performs async load operations after the view is loaded.
        /// </summary>
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        protected virtual async Task LoadAsync()
        {
            if (!IsAuthorized)
            {
                return;
            }

            await Task.Delay(1); // TODO: fix, busy is not shown without it
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            try
            {
                await LoadSessionDataAsync();
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

        private async Task LoadSessionDataAsync()
        {
            var accessToken = _session.GetAccessToken();

            await LoadUserResourcesAsync(accessToken);

            await LoadUserProfileAsync(accessToken);
        }

        private async Task LoadUserProfileAsync(string accessToken)
        {
            var profile = new UserProfile();
            IModelReader pReader = new HttpReader(_resources.ProfileUrl, accessToken);
            await profile.LoadAsync(pReader);

            Profile = profile;
        }

        private async Task LoadUserResourcesAsync(string accessToken)
        {
            _resources = new UserResources();
            IModelReader rReader = new HttpReader(Urls.UserResourcesUrl, accessToken);
            await _resources.LoadAsync(rReader);
        }

        private RelayCommand _connectComand;
        /// <summary>
        /// Connects async to RunKeeper.
        /// </summary>
        public RelayCommand ConnectCommand => _connectComand ?? 
            (_connectComand = new RelayCommand(async () => await ConnectAsync()));

        /// <summary>
        /// ConnectCommand handler
        /// </summary>
        /// <returns>Awaitable task</returns>
        protected virtual async Task ConnectAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            try
            {
                await _session.AuthorizeAsync();
                IsAuthorized = _session.IsAuthorized();
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
        /// <summary>
        /// Disconnects existent session async from RunKeeper.
        /// </summary>
        public RelayCommand DisconnectCommand => _disconnectComand ??
            (_disconnectComand = new RelayCommand(async () => await DisconnectAsync()));

        /// <summary>
        /// DisconnectCommand handler
        /// </summary>
        /// <returns>Awaitable task</returns>
        protected virtual async Task DisconnectAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            try
            {
                await _session.UnauthorizeAsync();
                IsAuthorized = _session.IsAuthorized();
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