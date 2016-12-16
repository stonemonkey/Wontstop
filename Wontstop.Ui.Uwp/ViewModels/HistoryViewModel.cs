// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
    public class HistoryViewModel : IHandle<BusyMessage>
    {
        public string Title => "History";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public History History { get; }

        public ActivityHistoryItemDto SelectedItem { get; set; }

            private readonly UserResources _userResources;

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public HistoryViewModel(
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
                SetSelected();
            }
            catch (Exception exception)
            {
                _eventAggregator.PublishOnCurrentThread(exception);
            }
            finally
            {
                Busy = false;
                Empty = (History.Items == null) || !History.Items.Any();
            }
        }

        private static string _lastOpenedItemId;

        private void SetSelected()
        {
            if (_lastOpenedItemId != null)
            {
                SelectedItem = History.Items
                    .SelectMany(x => x)
                    .FirstOrDefault(x => string.Equals(
                        _lastOpenedItemId, x.ResourcePath, StringComparison.Ordinal));

                _eventAggregator.PublishOnCurrentThread(new ScrollMessage {Item = SelectedItem});
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
            (_itemClickComand = new RelayCommand<ActivityHistoryItemDto>(Open));

        protected virtual void Open(ActivityHistoryItemDto item)
        {
            _lastOpenedItemId = item.ResourcePath;

            _navigationService.Navigate(typeof(ActivityPage), _lastOpenedItemId);
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
