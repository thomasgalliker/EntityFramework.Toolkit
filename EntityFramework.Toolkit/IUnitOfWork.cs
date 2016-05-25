using System.Threading.Tasks;
#if !NET40
using System.Threading.Tasks;
#endif

namespace System.Data.Extensions
{
    public interface IUnitOfWork : IDisposable
    {
        void RegisterContext<TContext>(TContext contextFactory) where TContext : IContext;

        /// <summary>
        /// Saves pending changes to all registered contexts.
        /// </summary>
        /// <returns>The total number of objects committed.</returns>
        int Commit();

#if !NET40
        /// <summary>
        /// Saves pending changes to all registered contexts.
        /// </summary>
        /// <returns>The total number of objects committed.</returns>
        Task<int> CommitAsync();
#endif
    }
}
