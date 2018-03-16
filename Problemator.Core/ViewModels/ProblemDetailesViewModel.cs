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

        public bool Busy { get; private set; }

        public int TriesCount { get; set; }
     
        public WallProblem Problem { get; private set; }

        public void Activate(object parameter)
        {
            Problem = (WallProblem) parameter;
        }
    }
}