// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using RunKeeper.WinRT.HealthGraph.Authorization;
using RunKeeper.WinRT.HealthGraph.Infrastructure;
using RunKeeper.WinRT.HealthGraph.User;

namespace Wontstop.Ui.Uwp.ViewModels
{
    public class AccountViewModel : IHandle<BusyMessage>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Profile";

        public bool Busy { get; set; }

        public UserProfile Profile { get; }

        public AuthorizationSession Session { get; }

        private readonly UserResources _userResources;
        private readonly IEventAggregator _eventAggregator;

        public AccountViewModel(
            IEventAggregator eventAggregator,
            UserProfile userProfile,
            UserResources userResources,
            AuthorizationSession authorizationSession)
        {
            _eventAggregator = eventAggregator;
            Profile = userProfile;
            _userResources = userResources;
            Session = authorizationSession;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        protected async Task LoadAsync()
        {
            if (!Session.IsAuthorized)
            {
                return;
            }

            Busy = true;

            try
            {
                await LoadProfileAsync();
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

        private RelayCommand _connectComand;
        public RelayCommand ConnectCommand => _connectComand ??
            (_connectComand = new RelayCommand(async () => await ConnectAsync()));

        protected virtual async Task ConnectAsync()
        {
            Busy = true;

            try
            {
                await Session.AuthorizeAsync();
                await LoadProfileAsync();
            }
            catch (WebAuthenticationException)
            {
                // user canceled authorization or authorization failed because of network connection
                // we eat it since user has already informed about the error in the SSO page 
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

        private async Task LoadProfileAsync()
        {
            await _userResources.LoadAsync();
            Profile.SetResource(_userResources.Profile);
            await Profile.LoadAsync();
        }

        private RelayCommand _disconnectComand;
        public RelayCommand DisconnectCommand => _disconnectComand ??
            (_disconnectComand = new RelayCommand(async () => await DisconnectAsync()));

        protected virtual async Task DisconnectAsync()
        {
            Busy = true;

            try
            {
                await Session.UnauthorizeAsync();

                await _userResources.ClearAsync();
                await Profile.ClearAsync();
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

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
            Debug.WriteLine($"BUSY {Busy}");
        }
    }
}