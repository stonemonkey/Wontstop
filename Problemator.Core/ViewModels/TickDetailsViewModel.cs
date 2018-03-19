// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TickDetailsViewModel : 
        IHandle<TickAddedMesage>, 
        IActivable, 
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy { get; private set; }
        public bool IsDirty { get; private set; }

        public Tick Tick { get; private set; }

        private int _triesCount;
        public int TriesCount
        {
            get { return _triesCount; }
            set
            {
                _triesCount = value;
                UpdateDirty();
            }
        }

        private Grade _selectedGrade;
        public Grade SelectedGrade
        {
            get { return _selectedGrade; }
            set
            {
                _selectedGrade = value;
                UpdateDirty();
            }
        }

        public IList<Grade> Grades { get; private set; }

        private string _selectedAscentType;
        public string SelectedAscentType
        {
            get { return _selectedAscentType; }
            set
            {
                _selectedAscentType = value;
                UpdateDirty();
            }
        }

        private TimeSpan _sendTime;
        public TimeSpan SendTime
        {
            get { return _sendTime; }
            set
            {
                _sendTime = value;
                UpdateDirty();
            }
        }

        private DateTime SendTimestamp => 
            (Tick.Timestamp.Date + SendTime).AddSeconds(Tick.Timestamp.Second);

        private void UpdateDirty()
        {
            if (Tick == null || 
                TriesCount == 0 ||
                SelectedGrade == null ||
                SelectedAscentType == null)
            {
                return;
            }

            IsDirty = Tick.Tries != TriesCount ||
                Tick.Timestamp != SendTimestamp ||
                Tick.AscentTypeId != _session.GetSportAscentTypeId(SelectedAscentType) ||
                Tick.GradeOpinionId == null && Tick.GradeId != Grades.GetByName(SelectedGrade.Name).Id ||
                Tick.GradeOpinionId != null && Tick.GradeOpinionId != Grades.GetByName(SelectedGrade.Name).Id;
        }

        public IList<string> AscentTypes { get; private set; }

        public ProblemDetails Problem { get; private set; }

        private readonly Ticks _ticks;
        private readonly Problem _problem;
        private readonly Session _session;
        private readonly Sections _sections;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public TickDetailsViewModel(
            Ticks ticks,
            Problem problem,
            Session session,
            Sections sections,
            IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _ticks = ticks;
            _problem = problem;
            _session = session;
            _sections = sections;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        public void Activate(object parameter)
        {
            Tick = (Tick) parameter;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            Busy = true;

            await LoadSessionAsync();
            UpdateFieldsFromTick();

            await LoadDetailsAsync();

            IsDirty = false;
            Busy = false;
        }

        private async Task LoadSessionAsync()
        {
            await _session.LoadAsync(false);

            Grades = await _session.GetGradesAsync();
            AscentTypes = _session.GetSportAscentTypes();
            SelectedAscentType = await _session.GetUserSportAscentType();
        }

        private void UpdateFieldsFromTick()
        {
            TriesCount = Tick.Tries;
            SendTime = Tick.Timestamp.TimeOfDay;
            SelectedAscentType = _session.GetSportAscentType(Tick.AscentTypeId);
            SelectedGrade = Grades.GetById(Tick.GradeOpinionId ?? Tick.GradeId);
        }

        private async Task LoadDetailsAsync()
        {
            Problem = await _problem.GetDetailsAsync(Tick.ProblemId);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _saveComand;
        public RelayCommand SaveCommand => _saveComand ??
            (_saveComand = new RelayCommand(async () => await SaveAsync(), () => IsDirty && !Busy));

        private async Task SaveAsync()
        {
            Busy = true;

            Tick.Tries = TriesCount;
            Tick.Timestamp = SendTimestamp;
            Tick.AscentTypeId = _session.GetSportAscentTypeId(SelectedAscentType);
            Tick.GradeOpinionId = Grades.GetByName(SelectedGrade.Name).Id;

            await _ticks.SaveTickAsync(Tick);

            Busy = false;
        }

        public void Handle(TickAddedMesage message)
        {
            _navigationService.GoBack();
        }
    }
}