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
        /// Saves all pending changes.
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();

#if !NET40
        /// <summary>
        /// Saves all pending changes asynchronously.
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        Task<int> CommitAsync();
#endif
    }
}
