// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mvvm.WinRT;
using Mvvm.WinRT.AttachedProperties;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using Windows.UI.Xaml.Navigation;
using Wontstop.Climb.Ui.Uwp.Dtos;
using Wontstop.Climb.Ui.Uwp.Views;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemDetailesViewModel : IActivable
    {
        public int TriesCount
        {
            get
            {
                return Problem.Tick.Tries;
            }
            set
            {
                Problem.Tick.Tries = value;
            }
        }

        public Problem Problem { get; private set; }

        public void Activate(object parameter)
        {
            var navigationEventArgs = (NavigationEventArgs) parameter;
            Problem = (Problem) navigationEventArgs.Parameter;
        }

        private const int MaxTriesCount = 100;

        private RelayCommand _incrementTriesCountComand;
        public RelayCommand IncrementCommand => _incrementTriesCountComand ??
            (_incrementTriesCountComand = new RelayCommand(IncrementTriesCount, () => TriesCount < MaxTriesCount));

        private void IncrementTriesCount()
        {
            TriesCount++;
        }

        private RelayCommand _decrementTriesCountComand;
        public RelayCommand DecrementCommand => _decrementTriesCountComand ??
            (_decrementTriesCountComand = new RelayCommand(DecrementTriesCount, () => TriesCount > 1));

        private void DecrementTriesCount()
        {
            TriesCount--;
        }
    }

    [ImplementPropertyChanged]
    public class ProblemItemViewModel
    {
        [Model]
        public Problem Problem { get; set; }

        public string RouteTypeText
        {
            get
            {
                if (Problem != null)
                {
                    return Problem.RouteType;
                }
                return null;
            }
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public ProblemItemViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(Load));

        private void Load()
        {
            _eventAggregator.Subscribe(this);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _showProblemDetailes;
        public RelayCommand ShowProblemDetailesCommand => _showProblemDetailes ??
            (_showProblemDetailes = new RelayCommand(ShowProblemDetailes));

        private void ShowProblemDetailes()
        {
            _navigationService.Navigate(typeof (ProblemDetailesPage), Problem);
        }
    }
}