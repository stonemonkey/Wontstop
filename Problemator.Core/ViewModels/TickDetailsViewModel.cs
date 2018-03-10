// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TickDetailsViewModel : INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        private Tick _tick;
        [Model]
        public Tick Tick
        {
            get { return _tick; }
            set
            {
                _tick = value;
                IsDirty = false;
                TryUpdateTickSelectedValues();
            }
        }

        public bool IsDirty { get; private set; }

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

        private void UpdateDirty()
        {
            if (_tick == null || 
                TriesCount == 0 || SelectedAscentType == null || SelectedGrade == null)
            {
                return;
            }

            IsDirty = _tick.Tries != TriesCount ||
                _tick.AscentTypeId != _session.GetSportAscentTypeId(SelectedAscentType) ||
                _tick.GradeOpinionId == null && _tick.GradeId != Grades.GetByName(SelectedGrade.Name).Id ||
                _tick.GradeOpinionId != null && _tick.GradeOpinionId != Grades.GetByName(SelectedGrade.Name).Id;
        }

        public IList<string> AscentTypes { get; private set; }

        private readonly Session _session;
        private readonly Sections _sections;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TickDetailsViewModel(
            Session session,
            Sections sections,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _session = session;
            _sections = sections;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            await _session.LoadAsync(false);

            Grades = await _session.GetGradesAsync();
            AscentTypes = _session.GetSportAscentTypes();
            SelectedAscentType = await _session.GetUserSportAscentType();

            TryUpdateTickSelectedValues();
        }

        private void TryUpdateTickSelectedValues()
        {
            if (_tick != null && _session.IsLoaded() && Grades != null)
            {
                TriesCount = _tick.Tries;
                SelectedAscentType = _session.GetSportAscentType(_tick.AscentTypeId);
                SelectedGrade = Grades.GetById(_tick.GradeOpinionId ?? _tick.GradeId);
            }
        }

        #region Tick

        private RelayCommand _saveComand;

        public RelayCommand SaveCommand => _saveComand ??
            (_saveComand = new RelayCommand(async () => await SaveAsync()));

        private async Task SaveAsync()
        {
            _eventAggregator.PublishShowBusy();

            await _requestsFactory.CreateDeleteTickRequest(_tick.Id)
                .RunAsync<ProblematorJsonParser>();

            _tick.Tries = TriesCount;
            _tick.AscentTypeId = _session.GetSportAscentTypeId(SelectedAscentType);
            _tick.GradeOpinionId = Grades.GetByName(SelectedGrade.Name).Id;

            var response = await _requestsFactory.CreateUpdateTickRequest(Tick)
                .RunAsync<ProblematorJsonParser>();

            string[] failedToAddTags = new string[] { };
            if (response.IsSuccessfull())
            {
                IsDirty = true;
            }
            else
            {
                failedToAddTags = new string[] { Tick.TagShort };
            }
            _eventAggregator.PublishFailedToAdd(failedToAddTags);

            _eventAggregator.PublishHideBusy();
        }

        #endregion
    }
}