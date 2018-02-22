// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using RunKeeper.WinRT.HealthGraph.Activities;
using System.ComponentModel;

namespace Wontstop.Ui.Uwp.ViewModels
{
    public class ActivityViewModel : IActivable, IHandle<BusyMessage>, INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy { get; private set; }

        public Activity Activity { get; }

        private readonly IEventAggregator _eventAggregator;

        public ActivityViewModel(
            IEventAggregator eventAggregator,
            Activity activity)
        {
            _eventAggregator = eventAggregator;
            Activity = activity;
        }

        public void Activate(object parameter)
        {
            var navigationEventArgs = (NavigationEventArgs) parameter;
            var resource = navigationEventArgs?.Parameter?.ToString();
            if (resource != null)
            {
                Activity.SetResource(resource);
            }
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
            await Activity.LoadAsync();
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
