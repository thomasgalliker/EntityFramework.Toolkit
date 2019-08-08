﻿using System;
using System.Collections.Generic;

#if !NET40
using System.Threading.Tasks;
#endif

namespace EntityFramework.Toolkit.Core
{
    public interface IUnitOfWork : IDisposable
    {
        void RegisterContext<TContext>(TContext contextFactory) where TContext : IContext;

        /// <summary>
        /// Saves pending changes to all registered contexts.
        /// </summary>
        /// <returns>The total number of objects committed.</returns>
        ICollection<ChangeSet> Commit();

#if !NET40
        /// <summary>
        /// Saves pending changes to all registered contexts.
        /// </summary>
        /// <returns>The total number of objects committed.</returns>
        Task<ICollection<ChangeSet>> CommitAsync();
#endif
    }
}
