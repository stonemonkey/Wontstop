using System;
using System.Threading.Tasks;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Authorization;

namespace Wontstop.Ui.Uwp.ViewModels
{
    /// <summary>
    /// Activities view model
    /// </summary>
    [ImplementPropertyChanged]
    public class ActivitiesViewModel : IHandle<BusyMessage>
    {
        /// <summary>
        /// Specifies if a background operation is executed in the background (async) therefore 
        /// Busy indicator should be shown to the user.
        /// </summary>
        public bool Busy { get; private set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly AuthorizationSession _authorizationSession;

        public ActivitiesViewModel(
            IEventAggregator eventAggregator, 
            AuthorizationSession authorizationSession)
        {
            _eventAggregator = eventAggregator;
            _authorizationSession = authorizationSession;
        }

        private RelayCommand _loadComand;
        /// <summary>
        /// Performs async load operations after the view is loaded.
        /// </summary>
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));
        
        /// <summary>
        /// LoadCommand handler
        /// </summary>
        /// <returns>Awaitable task</returns>
        protected virtual async Task LoadAsync()
        {
            Busy = true;

            try
            {
                await LoadActivitiesDataAsync();
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

        private Task LoadActivitiesDataAsync()
        {
            return Task.FromResult(true);
        }

        private RelayCommand _unloadComand;
        /// <summary>
        /// Performs async save operations after view is uloaded.
        /// </summary>
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(async () => await UnloadAsync()));
        
        /// <summary>
        /// UnloadCommand handler
        /// </summary>
        /// <returns>Awaitable task</returns>
        protected virtual Task UnloadAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Toggles busy indicator flag based on messages coming from other view models.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
