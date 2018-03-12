// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
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
        private List<WallProblem> _problems;
        private IDictionary<string, WallSection> _sections;

        public bool HasProblems() => _problems != null && _problems.Any();

        public async Task LoadAsync()
        {
            (await _requestsFactory.CreateWallSectionsRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        _sections = p.To<IDictionary<string, WallSection>>() ?? 
                            new Dictionary<string, WallSection>();
                        _problems = _sections.Values
                            .SelectMany(x => x.Problems)
                            .ToList();
                        _tags = _problems
                            .Select(x => x.TagShort)
                            .ToList();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        public IList<WallSection> Get() => _sections.Values.ToList();

        public bool ContainsProblem(string tag) =>
            _tags.Contains(tag, StringComparer.OrdinalIgnoreCase);

        public WallProblem GetFirstAvailableProblem(string tag, DateTime date) =>
            _problems.First(x =>
                IsAvailaleAt(x, date) &&
                x.TagShort.Equals(tag, StringComparison.OrdinalIgnoreCase));

        public IList<WallProblem> GetAvailableProblems(string tagFragment, DateTime date) =>
            _problems.Where(x =>
                    IsAvailaleAt(x, date) &&
                    x.TagShort.StartsWith(tagFragment, StringComparison.OrdinalIgnoreCase))
                .ToList();

        private bool IsAvailaleAt(WallProblem problem, DateTime date)
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
