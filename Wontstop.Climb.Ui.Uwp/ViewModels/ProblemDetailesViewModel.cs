// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mvvm.WinRT;
using Mvvm.WinRT.Commands;
using PropertyChanged;
using Windows.UI.Xaml.Navigation;
using Wontstop.Climb.Ui.Uwp.Dtos;

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
}