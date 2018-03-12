// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TagProblemsViewModel : 
        IHandle<LocationChangedMessage>,
        IHandle<DayChangedMessage>,
        IHandle<TickAddMesage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public Grade SelectedGrade { get; set; }
        public IList<Grade> Grades { get; private set; }

        public string SelectedAscentType { get; set; }
        public IList<string> AscentTypes { get; private set; }

        public bool AreProblemsMissing { get; private set; }

        private bool _busy;
        private DateTime _selectedDate;

        private readonly Ticks _ticks;
        private readonly Session _session;
        private readonly Sections _sections;
        private readonly ITimeService _timeService;
        private readonly IEventAggregator _eventAggregator;

        public TagProblemsViewModel(
            Ticks ticks,
            Session session,
            Sections sections,
            ITimeService timeService,
            IEventAggregator eventAggregator)
        {
            _ticks = ticks;
            _session = session;
            _sections = sections;
            _timeService = timeService;
            _eventAggregator = eventAggregator;

            _selectedDate = _timeService.Now;
        }

        #region Load/Unload handlers

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            ClearTaggedProblem();

            await LoadSessionAsync();
            await LoadSectionsAsync();
        }

        private async Task LoadSessionAsync()
        {
            await _session.LoadAsync(false);

            Grades = await _session.GetGradesAsync();
            AscentTypes = _session.GetSportAscentTypes();
            SelectedAscentType = await _session.GetUserSportAscentType();
        }

        private async Task LoadSectionsAsync()
        {
            await _sections.LoadAsync();

            AreProblemsMissing = !_sections.HasProblems();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        #endregion

        #region Suggestions

        public bool CanTick { get; private set; }

        public string Tag { get; set; }

        public IList<WallProblem> SuggestedProblems { get; set; }

        private RelayCommand<bool> _tagChangedComand;

        public RelayCommand<bool> TagsChangedCommand => _tagChangedComand ??
            (_tagChangedComand = new RelayCommand<bool>(TagChanged));

        private const int TagMinLength = 2;

        private void TagChanged(bool byUser)
        {
            if (!byUser || _busy || string.IsNullOrWhiteSpace(Tag))
            {
                CanTick = false;
                return;
            }
            CanTick = IsAValidTag(Tag);

            if (Tag.Length < TagMinLength)
            {
                SuggestedProblems = null;
                return;
            }
            if (CanTick)
            {
                SetTaggedProblem(Tag);
                return;
            }

            SuggestedProblems = _sections.GetAvailableProblems(Tag, _selectedDate);
        }

        private RelayCommand<string> _suggestionChosenComand;

        public RelayCommand<string> SuggestionChosenCommand => _suggestionChosenComand ??
            (_suggestionChosenComand = new RelayCommand<string>(
                SuggestionChosen, tag => !_busy && !string.IsNullOrWhiteSpace(tag)));

        private void SuggestionChosen(string tag)
        {
            Tag = tag;
            SetTaggedProblem(tag);
        }

        private WallProblem _taggedProblem;

        private void ClearTaggedProblem()
        {
            Tag = null;
            _taggedProblem = null;
        }

        private void SetTaggedProblem(string tag)
        {
            if (_taggedProblem != null &&
                string.Equals(tag, _taggedProblem.TagShort, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _taggedProblem = _sections.GetFirstAvailableProblem(tag, _selectedDate);
            SelectedGrade = Grades.GetById(_taggedProblem.GradeId);
        }

        private bool IsAValidTag(string tag)
        {
            return _sections.ContainsProblem(tag);
        }

        #endregion

        #region Tick

        public int TriesCount { get; set; }

        private RelayCommand _tickComand;

        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(async () => await TickAsync()));

        private async Task TickAsync()
        {
            _eventAggregator.PublishShowBusy();

            await _ticks.AddTickAsync(CreateTick());
            SuggestedProblems = null;

            _eventAggregator.PublishHideBusy();
        }

        private Tick CreateTick()
        {
            var problemId = _taggedProblem.ProblemId;
            var ascentTypeId = _session.GetSportAscentTypeId(SelectedAscentType);
            var gradeOpinionId = SelectedGrade.Id;

            return new Tick
            {
                Tag = Tag,
                Tries = TriesCount,
                Timestamp = _selectedDate,
                ProblemId = problemId,
                AscentTypeId = ascentTypeId,
                GradeOpinionId = gradeOpinionId,
            };
        }

        #endregion

        #region Message handlers

        public async void Handle(LocationChangedMessage message)
        {
            ClearTaggedProblem();

            await LoadSectionsAsync();
        }

        public void Handle(DayChangedMessage message)
        {
            ClearTaggedProblem();

            _selectedDate = message.NewDay.Date;
        }

        public async void Handle(TickAddMesage message)
        {
            ClearTaggedProblem();

            await _sections.LoadAsync();
        }

        public void Handle(BusyMessage message)
        {
            _busy = message.Show;
        }

        #endregion
    }
}