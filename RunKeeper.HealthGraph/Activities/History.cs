// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    [ImplementPropertyChanged]
    public class History
    {
        public IList<IGrouping<string, ActivityHistoryItemDto>> Items { get; private set; }

        private readonly IModelRepository _serverRepository;

        public History(IModelRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        private string _resource;

        public void SetResource(string resource)
        {
            _resource = resource;
        }

        public async Task LoadAsync()
        {
            var nextPagePath = _resource;
            var history = new List<ActivityHistoryItemDto>();
            do
            {
                var page = await _serverRepository.ReadAsyc<ActivityHistoryPageDto>(nextPagePath);
                history.AddRange(page.Items);
                nextPagePath = page.NextPageUrl;
            } while (nextPagePath != null);

            Items = history.GroupBy(x => GetMonthGroupNameFor(x.StartTime))
                .ToList();
        }

        private static string GetMonthGroupNameFor(DateTime startTime)
        {
            return startTime.ToString("MMMM yy");
        }
    }
}