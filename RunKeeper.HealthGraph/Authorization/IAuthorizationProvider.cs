using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    public interface IAuthorizationProvider
    {
        Task<T> AuthorizeAsync<T>() where T : class;
        Task DeauthorizeAsync(string token);
    }
}