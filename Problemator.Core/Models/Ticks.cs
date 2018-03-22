// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using MvvmToolkit.Messages;
using MvvmToolkit.Utils;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Problemator.Core.Models
{
    public class Ticks
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public Ticks(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        public async Task<IList<DateTimeOffset>> GetTickDatesAsync()
        {
            IList<DateTimeOffset> dates = null;

            (await _requestsFactory.CreateTickDatesRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        dates = p.To<IList<DateTimeOffset>>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);

            return dates;
        }

        public async Task<IList<Tick>> GetDayTicksAsync(DateTime day)
        {
            DayTicks dayTicks = null;

            (await _requestsFactory.CreateDayTicksRequest(day)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        dayTicks = p.To<DayTicks>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);

            return dayTicks.Ticks;
        }

        public async Task<IList<Tick>> GetProblemTicksAsync(string problemId)
        {
            IDictionary<string, Tick> problemTicks = null;

            (await _requestsFactory.CreateProblemTicksRequest(problemId)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        problemTicks = p.To<IDictionary<string, Tick>>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);

            return problemTicks.Values.ToList();
        }

        public async Task AddTickAsync(Tick tick)
        {
            tick.ValidateNotNull(nameof(tick));

            (await _requestsFactory.CreateUpdateTickRequest(tick)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        _eventAggregator.PublishAddedTick(tick.GetTagShort());
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        public async Task SaveTickAsync(Tick tick)
        {
            tick.ValidateNotNull(nameof(tick));

            await _requestsFactory.CreateDeleteTickRequest(tick.Id)
                .RunAsync<ProblematorJsonParser>();

            await AddTickAsync(tick);
        }

        public async Task DeleteTickAsync(Tick tick)
        {
            tick.ValidateNotNull(nameof(tick));

            (await _requestsFactory.CreateDeleteTickRequest(tick.Id)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        _eventAggregator.PublishRemovedTick(tick);
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }
    }
}
