#if !NET40
using System.Threading.Tasks;
#endif

namespace EntityFramework.Toolkit.EF6.Contracts
{
    public interface IAuditContext : IContext
    {
        /// <summary>
        ///     Specifies if the auditing feature is enabled.
        /// </summary>
        bool AuditEnabled { get; }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the context has been disposed.</exception>
        ChangeSet SaveChanges(string username);

#if !NET40
        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the context has been disposed.</exception>
        Task<ChangeSet> SaveChangesAsync(string username);
#endif
    }
}