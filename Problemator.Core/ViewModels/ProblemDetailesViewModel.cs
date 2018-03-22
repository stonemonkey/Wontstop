// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Models;

namespace Problemator.Core.ViewModels
{
    public class ProblemDetailesViewModel : IActivable, INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy { get; private set; }

        public WallProblem Problem { get; private set; }

        public ProblemDetails Details { get; private set; }

        private Problem _problem;
        private IEventAggregator _eventAggregator;

        public ProblemDetailesViewModel(
            Problem problem,
            IEventAggregator eventAggregator)
        {
            _problem = problem;
            _eventAggregator = eventAggregator;
        }

        public void Activate(object parameter)
        {
            Problem = (WallProblem) parameter;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            Busy = true;

            await LoadDetailsAsync();

            Busy = false;
        }

        private async Task LoadDetailsAsync()
        {
            Details = await _problem.GetDetailsAsync(Problem.ProblemId);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}