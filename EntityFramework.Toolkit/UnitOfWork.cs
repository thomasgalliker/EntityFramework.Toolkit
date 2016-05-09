using System.Collections.Generic;
using System.Transactions;

namespace System.Data.Extensions
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, IContext> contexts;

        private bool disposed;

        public UnitOfWork()
        {
            this.contexts = new Dictionary<Type, IContext>();
        }

        public void RegisterContext<TContext>(TContext contextFactory) where TContext : IContext
        {
            //Guard.ArgumentNotNull(() => contextFactory);

            Type contextType = contextFactory.GetType();

            if (!this.contexts.ContainsKey(contextType))
            {
                this.contexts.Add(contextType, contextFactory);
            }
        }

        public int Commit()
        {
            int numberOfChanges = 0;

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = Transactions.IsolationLevel.Serializable }))
            {
                foreach (var context in this.contexts.Values)
                {
                    numberOfChanges += context.SaveChanges();
                }

                transactionScope.Complete();
            }

            return numberOfChanges;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    foreach (var c in this.contexts.Values)
                    {
                        c.Dispose();
                    }
                    this.contexts.Clear();
                }

                this.disposed = true;
            }
        }

        ~UnitOfWork()
        {
            this.Dispose(false);
        }
    }
}