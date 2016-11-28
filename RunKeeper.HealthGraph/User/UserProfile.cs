using System.Collections.Generic;
using System.Threading.Tasks;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.User
{
    [ImplementPropertyChanged]
    public class UserProfile
    {
        public string Name { get; private set; }

        public string Location { get; private set; }

        public string Picture { get; private set; }

        private readonly IModelRepository _localRepository;
        private readonly IModelRepository _serverRepository;

        public UserProfile(
            IModelRepository localRepository, 
            IModelRepository serverRepository)
        {
            _localRepository = localRepository;
            _serverRepository = serverRepository;
        }

        private string _resource;

        public void SetResource(string resource)
        {
            _resource = resource;
        }

        private IDictionary<string, string> _data;

        public async Task LoadAsync()
        {
            _data = await _resource.FatchCachedAsync(_localRepository, _serverRepository);

            MapData();
        }

        private void MapData()
        {
            Name = _data.GetValue("name");
            Location = _data.GetValue("location");
            Picture = _data.GetValue("normal_picture");
        }

        public async Task ClearAsync()
        {
            ClearProperties();

            _data.Clear();
            await _localRepository.DeleteAsync(_resource);
        }

        private void ClearProperties()
        {
            Name = null;
            Location = null;
            Picture = null;
        }
    }
}
