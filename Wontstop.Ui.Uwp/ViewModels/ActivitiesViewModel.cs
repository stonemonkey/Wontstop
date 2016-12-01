// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Mvvm.WinRT;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Activities;
using RunKeeper.WinRT.HealthGraph.User;
using Wontstop.Ui.Uwp.Views;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ActivitiesViewModel : IHandle<BusyMessage>
    {
        public bool Busy { get; private set; }

        public History History { get; }

        private readonly UserResources _userResources;

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public ActivitiesViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            History history,
            UserResources userResources)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            History = history;
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

        private RelayCommand<ActivityHistoryItemDto> _itemClickComand;
        public RelayCommand<ActivityHistoryItemDto> ItemClickCommand => _itemClickComand ??
            (_itemClickComand = new RelayCommand<ActivityHistoryItemDto>(ItemClick));

        protected virtual void ItemClick(ActivityHistoryItemDto item)
        {
            _navigationService.Navigate(typeof(ActivityPage), item.ResourcePath);
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
