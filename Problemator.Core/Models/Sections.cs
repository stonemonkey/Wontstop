﻿using HttpApiClient;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Problemator.Core.Models
{
    public class Sections
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public Sections(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        private List<string> _tags;
        private List<Problem> _problems;
        private IDictionary<string, WallSection> _sections;

        public bool HasProblems() => _problems != null && _problems.Any();

        public async Task LoadAsync()
        {
            (await _requestsFactory.CreateWallSectionsRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleWallSectionsResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleWallSectionsResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            _sections = parser.To<IDictionary<string, WallSection>>() ?? new Dictionary<string, WallSection>();
            _problems = _sections.Values
                .SelectMany(x => x.Problems)
                .ToList();
            _tags = _problems
                .Select(x => x.TagShort)
                .ToList();
        }

        public IList<WallSection> Get() => _sections.Values.ToList();

        public bool ContainProblem(string tag) =>
            _tags.Contains(tag, StringComparer.OrdinalIgnoreCase);

        public Problem GetFirstAvailableProblem(string tag, DateTime date) =>
            _problems.First(x =>
                IsAvailaleAt(x, date) &&
                x.TagShort.Equals(tag, StringComparison.OrdinalIgnoreCase));

        public IList<Problem> GetAvailableProblems(string tagFragment, DateTime date) =>
            _problems.Where(x =>
                    IsAvailaleAt(x, date) &&
                    x.TagShort.StartsWith(tagFragment, StringComparison.OrdinalIgnoreCase))
                .ToList();

        private bool IsAvailaleAt(Problem problem, DateTime date)
        {
            if (problem.Removed == null)
            {
                return true;
            }

            var removed = DateTime.Parse(problem.Removed);

            return removed > date;
        }
    }
}
