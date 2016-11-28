using System.Collections.Generic;
using System.Threading.Tasks;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.User
{
    public class UserResources
    {
        public string Id => _data.GetValue("userID");
        public string Profile =>  _data.GetValue("profile");
        public string Settings => _data.GetValue("settings");
        public string Activities => _data.GetValue("fitness_activities");

        private readonly IModelRepository _localRepository;
        private readonly IModelRepository _serverRepository;

        private const string Resource = "/user";

        public UserResources(
            IModelRepository localRepository, 
            IModelRepository serverRepository)
        {
            _localRepository = localRepository;
            _serverRepository = serverRepository;
        }

        private IDictionary<string, string> _data;

        public async Task LoadAsync()
        {
            _data = await Resource.FatchCachedAsync(_localRepository, _serverRepository);
        }

        public async Task ClearAsync()
        {
            _data.Clear();
            await _localRepository.DeleteAsync(Resource);
        }
    }
}
