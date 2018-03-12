// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using MvvmToolkit;
using MvvmToolkit.Commands;
using Problemator.Core.Dtos;

namespace Problemator.Core.ViewModels
{
    public class ProblemDetailesViewModel : IActivable, INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public int TriesCount { get; set; }
     
        public WallProblem Problem { get; private set; }

        public void Activate(object parameter)
        {
            Problem = (WallProblem) parameter;
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