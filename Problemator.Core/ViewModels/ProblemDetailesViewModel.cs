// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MvvmToolkit;
using MvvmToolkit.Commands;
using Problemator.Core.Dtos;
using PropertyChanged;

namespace Problemator.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemDetailesViewModel : IActivable
    {
        public int TriesCount { get; set; }
     
        public Problem Problem { get; private set; }

        public void Activate(object parameter)
        {
            Problem = (Problem) parameter;
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