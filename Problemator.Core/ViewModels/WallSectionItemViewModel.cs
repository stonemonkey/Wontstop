// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;

namespace Problemator.Core.ViewModels
{
    public class WallSectionItemViewModel : 
        IHandle<BusyMessage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowProblems { get; set; }

        private WallSection _wallSection;
        [Model]
        public WallSection Section
        {
            get { return _wallSection; }
            set
            {
                _wallSection = value;
                TryUpdateFieldsFromSession();
            }
        }

        private bool _busy;

        private readonly Sections _sections;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public WallSectionItemViewModel(
            Sections sections,
            IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _sections = sections;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(Load));

        private void Load()
        {
            _eventAggregator.Subscribe(this);
            TryUpdateFieldsFromSession();
        }

        private void TryUpdateFieldsFromSession()
        {
            if (Section != null)
            {
                ShowProblems = _sections.IsSectionOpen(Section.FullName);
            }
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _toggleSectionCommand;
        public RelayCommand ToggleSectionCommand => _toggleSectionCommand ??
            (_toggleSectionCommand = new RelayCommand(ManageTicks, () => !_busy));

        private void ManageTicks()
        {
            _eventAggregator.PublishShowBusy();

            if (ShowProblems)
            {
                _sections.OpenSection(Section.FullName);
            }
            else
            {
                _sections.CloseSection(Section.FullName);
            }

            _eventAggregator.PublishHideBusy();
        }

        public void Handle(BusyMessage message)
        {
            _busy = message.IsBusy;
        }
    }
}