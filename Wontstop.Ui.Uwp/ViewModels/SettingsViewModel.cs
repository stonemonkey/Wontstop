using System.Threading.Tasks;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;

namespace Wontstop.Ui.Uwp.ViewModels
{
    /// <summary>
    /// Application settings view model
    /// </summary>
    [ImplementPropertyChanged]
    public class SettingsViewModel : IHandle<BusyMessage>
    {
        /// <summary>
        /// Busy indicator flag, true = on, false = off
        /// </summary>
        public bool Busy { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        private RelayCommand _loadComand;
        /// <summary>
        /// Performs async load operations after the view is loaded.
        /// </summary>
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        protected virtual Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            return Task.FromResult(true);
        }

        private RelayCommand _unloadComand;
        /// <summary>
        /// Performs async save operations after view is uloaded.
        /// </summary>
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(async () => await UnloadAsync()));

        protected virtual Task UnloadAsync()
        {
            _eventAggregator.Unsubscribe(this);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Toggles busy indicator flag based on messages coming from child view models. 
        /// </summary>
        /// <param name="message"></param>
        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
