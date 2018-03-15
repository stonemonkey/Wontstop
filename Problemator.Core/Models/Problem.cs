// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using MvvmToolkit.Messages;
using MvvmToolkit.Utils;
using Problemator.Core.Dtos;
using Problemator.Core.Utils;
using System.Threading.Tasks;

namespace Problemator.Core.Models
{
    public class Problem
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public Problem(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        public async Task<ProblemDetails> GetDetailsAsync(string id)
        {
            id.ValidateNotNullEmptyWhiteSpace(nameof(id));

            ProblemDetails problem = null;

            (await _requestsFactory.CreateProblemRequest(id)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        var data = p.GetData();
                        problem = data["problem"].ToObject<ProblemDetails>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);

            return problem;
        }
    }
}
