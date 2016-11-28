using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public interface IReadOnlyBusinessModel
    {
        Task LoadAsync(IModelRepository repository);
    }
}