using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    /// <summary>
    /// Authorization provider abstraction used to perform authorization against external services
    /// e.g. Facebook, Google.
    /// </summary>
    public interface IAuthorizationProvider
    {
        /// <summary>
        /// Starts the authorization process.
        /// </summary>
        /// <typeparam name="T">The type of data returned after a successfull authorization</typeparam>
        /// <returns>Awaitable task</returns>
        Task<T> AuthorizeAsync<T>() where T : class;

        /// <summary>
        /// Removes authorization from an external service.
        /// </summary>
        /// <param name="token">The token obtained when authorized</param>
        /// <returns>Awaitable task</returns>
        Task DeauthorizeAsync(string token);
    }
}