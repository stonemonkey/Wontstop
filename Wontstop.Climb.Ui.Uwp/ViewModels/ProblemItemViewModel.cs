// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mvvm.WinRT.AttachedProperties;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using Wontstop.Climb.Ui.Uwp.Dtos;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemItemViewModel
    {
        [Model]
        public Problem Problem { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public ProblemItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
    }
}