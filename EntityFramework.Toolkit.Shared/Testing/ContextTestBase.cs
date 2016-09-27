using System;
using System.Data.Entity;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit.Testing
{
    public abstract class ContextTestBase<TContext> : IDisposable
        where TContext : DbContextBase<TContext>
    {
        private readonly IDbConnection dbConnection;
        private readonly IDatabaseInitializer<TContext> databaseInitializer;

        [Obsolete("Use CreateContext instead.")]
        protected TContext Context
        {
            get
            {
                return this.CreateContext();
            }
        }

        protected bool DeleteDatabaseOnDispose { get; set; }

        protected ContextTestBase(Func<IDbConnection> dbConnection, bool deleteDatabaseOnDispose, Action<string> log = null)
            : this(dbConnection: dbConnection, databaseInitializer: null, log: log, deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, IDatabaseInitializer<TContext> initializer, Action<string> log = null)
            : this(dbConnection: dbConnection, databaseInitializer: initializer, log: log)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextTestBase{TContext}" /> class.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection" /> which is used to connect to the database.</param>
        /// <param name="log">Log delegate used to write diagnostic log messages to.</param>
        /// <param name="databaseInitializer">
        ///     The <see cref="IDatabaseInitializer{TContext}" /> which is used initialize the
        ///     database. (Default is <see cref="DropCreateDatabaseAlways{TContext}" />).
        /// </param>
        /// <param name="deleteDatabaseOnDispose">Determines if the database needs to be deleted on dispose. (Default is true).</param>
        protected ContextTestBase(Func<IDbConnection> dbConnection, Action<string> log = null, IDatabaseInitializer<TContext> databaseInitializer = null, bool deleteDatabaseOnDispose = true)
        {
            this.dbConnection = dbConnection();
            this.Log = log;
            this.DeleteDatabaseOnDispose = deleteDatabaseOnDispose;
            this.databaseInitializer = databaseInitializer ?? new DropCreateDatabaseAlways<TContext>();
        }

        public Action<string> Log { get; set; }

        /// <summary>
        ///     Returns the default database initializer (given by ctor) if <paramref name="databaseInitializer" /> is null.
        /// </summary>
        private IDatabaseInitializer<TContext> EnsureDatabaseInitializer(IDatabaseInitializer<TContext> databaseInitializer)
        {
            if (databaseInitializer == null)
            {
                databaseInitializer = this.databaseInitializer;
            }

            return databaseInitializer;
        }

        /// <summary>
        ///     Returns the default db connection (given by ctor) if <paramref name="dbConnection" /> is null.
        /// </summary>
        private IDbConnection EnsureDbConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                dbConnection = this.dbConnection;
            }

            return dbConnection;
        }
        protected TContext CreateContext()
        {
            return this.CreateContext(this.dbConnection, this.databaseInitializer);
        }

        protected TContext CreateContext(IDatabaseInitializer<TContext> databaseInitializer)
        {
            return this.CreateContext(this.dbConnection, databaseInitializer);
        }

        protected TContext CreateContext(IDbConnection dbConnection = null, IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            databaseInitializer = this.EnsureDatabaseInitializer(databaseInitializer);
            dbConnection = this.EnsureDbConnection(dbConnection);

            var contextType = typeof(TContext);
            return (TContext)Activator.CreateInstance(contextType, dbConnection, databaseInitializer, this.Log);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DeleteDatabaseOnDispose)
                {
                    using (var context = this.CreateContext())
                    {
                        context.DropDatabase();
                    }
                }
            }
        }
    }
}