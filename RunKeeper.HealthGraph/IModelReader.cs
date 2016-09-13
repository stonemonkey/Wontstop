using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph
{
    /// <summary>
    /// Reader abstraction used by the models to fetch their data from an external 
    /// device/source e.g. backend.
    /// </summary>
    public interface IModelReader
    {
        /// <summary>
        /// Fetches external data source.
        /// </summary>
        /// <typeparam name="T">Data type to be fetched</typeparam>
        /// <returns>Awaitable task</returns>
        Task<T> ReadAsyc<T>();
    }
}