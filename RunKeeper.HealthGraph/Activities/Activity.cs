using System;
using System.Threading.Tasks;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    [ImplementPropertyChanged]
    public class Activity
    {
        public string Type { get; private set; }
        public DateTime StartTime { get; private set; }
        public double Duration { get; private set; }
        public double TotalDistance { get; private set; }
        public int AverageHeartRate { get; private set; }

        private readonly IModelRepository _serverRepository;

        public Activity(IModelRepository serverRepository)
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
            var activity = await _serverRepository.ReadAsyc<ActivityDto>(_resource);
            AverageHeartRate = activity.AverageHeartRate;
            TotalDistance = activity.TotalDistance;
            Duration = activity.Duration;
            StartTime = activity.StartTime;
            Type = activity.Type;
        }
    }
}