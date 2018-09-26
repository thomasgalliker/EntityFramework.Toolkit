using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Toolkit.EF6.Contracts;
using EntityFramework.Toolkit.EF6.Extensions;

namespace EntityFramework.Toolkit.EF6.Testing
{
    /// <summary>
    /// ContextTestBase for context <typeparam name="TContext"/> using IDbConnection <typeparam name="TDbConnection"/>
    /// </summary>
    /// <typeparam name="TContext">The database context.</typeparam>
    /// <typeparam name="TDbConnection">The database connection.</typeparam>
    public abstract class ContextTestBase<TContext, TDbConnection> : ContextTestBase<TContext>
        where TContext : DbContextBase<TContext> where TDbConnection : IDbConnection, new()
    {
        protected ContextTestBase() : base(() => new TDbConnection())
        {
        }

        protected ContextTestBase(bool deleteDatabaseOnDispose) : base(() => new TDbConnection(), deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Action<string> log) : base(() => new TDbConnection(), log)
        {
        }

        protected ContextTestBase(Action<string> log, bool deleteDatabaseOnDispose) : base(() => new TDbConnection(), log, deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(IDatabaseInitializer<TContext> databaseInitializer) : base(() => new TDbConnection(), databaseInitializer)
        {
        }

        protected ContextTestBase(IDatabaseInitializer<TContext> databaseInitializer, Action<string> log) : base(() => new TDbConnection(), databaseInitializer, log)
        {
        }

        protected ContextTestBase(IDatabaseInitializer<TContext> databaseInitializer, Action<string> log, bool deleteDatabaseOnDispose) : base(() => new TDbConnection(), databaseInitializer, log, deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString) : base(dbConnectionString)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, bool deleteDatabaseOnDispose) : base(dbConnectionString, deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, Action<string> log) : base(dbConnectionString, log)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, Action<string> log, bool deleteDatabaseOnDispose) : base(dbConnectionString, log, deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, IDatabaseInitializer<TContext> databaseInitializer) : base(dbConnectionString, databaseInitializer)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log, bool deleteDatabaseOnDispose) : base(dbConnectionString, databaseInitializer, log, deleteDatabaseOnDispose)
        {
        }
    }

    public abstract class ContextTestBase<TContext> : IDisposable
        where TContext : DbContextBase<TContext>
    {
        private readonly ICollection<TContext> contextInstances = new List<TContext>();
        private readonly IDbConnection dbConnection;
        private readonly IDatabaseInitializer<TContext> databaseInitializer;
        private readonly string dbConnectionString;
        private bool disposed;

        protected ContextTestBase(Func<IDbConnection> dbConnection)
            : this(dbConnection: dbConnection, databaseInitializer: null, log: null)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, bool deleteDatabaseOnDispose)
            : this(dbConnection: dbConnection, databaseInitializer: null, log: null, deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, Action<string> log)
            : this(dbConnection: dbConnection, databaseInitializer: null, log: log, deleteDatabaseOnDispose: true)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, Action<string> log, bool deleteDatabaseOnDispose)
            : this(dbConnection: dbConnection, databaseInitializer: null, log: log, deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, IDatabaseInitializer<TContext> databaseInitializer)
            : this(dbConnection: dbConnection, databaseInitializer: databaseInitializer, log: null)
        {
        }

        protected ContextTestBase(Func<IDbConnection> dbConnection, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log)
            : this(dbConnection: dbConnection, databaseInitializer: databaseInitializer, log: log, deleteDatabaseOnDispose: true)
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
        protected ContextTestBase(Func<IDbConnection> dbConnection, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log, bool deleteDatabaseOnDispose)
        {
            this.dbConnection = dbConnection();
            this.Log = log;
            this.DeleteDatabaseOnDispose = deleteDatabaseOnDispose;
            this.databaseInitializer = databaseInitializer;
        }

        protected ContextTestBase(Func<string> dbConnectionString)
            : this(dbConnectionString: dbConnectionString, databaseInitializer: null)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, bool deleteDatabaseOnDispose)
            : this(dbConnectionString: dbConnectionString, databaseInitializer: null, log: null, deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, Action<string> log)
            : this(dbConnectionString: dbConnectionString, databaseInitializer: null, log: log, deleteDatabaseOnDispose: true)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, Action<string> log, bool deleteDatabaseOnDispose)
            : this(dbConnectionString: dbConnectionString, databaseInitializer: null, log: log, deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, IDatabaseInitializer<TContext> databaseInitializer)
            : this(dbConnectionString: dbConnectionString, databaseInitializer: databaseInitializer, log: null, deleteDatabaseOnDispose: true)
        {
        }

        protected ContextTestBase(Func<string> dbConnectionString, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log, bool deleteDatabaseOnDispose)
        {
            this.dbConnectionString = dbConnectionString();
            this.Log = log;
            this.DeleteDatabaseOnDispose = deleteDatabaseOnDispose;
            this.databaseInitializer = databaseInitializer;
        }

        public Action<string> Log { get; set; }

        protected bool DeleteDatabaseOnDispose { get; set; }

        /// <summary>
        ///     Returns the default database initializer (given by ctor) if <paramref name="databaseInitializer" /> is null.
        /// </summary>
        private IDatabaseInitializer<TContext> EnsureDatabaseInitializer(IDatabaseInitializer<TContext> databaseInitializer)
        {
            if (databaseInitializer == null)
            {
                databaseInitializer = this.databaseInitializer ?? new DropCreateDatabaseAlways<TContext>();
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

            if (dbConnection == null && string.IsNullOrEmpty(this.dbConnectionString))
            {
                throw new InvalidOperationException("Either dbConnection or nameOrConnectionString must be defined.");
            }

            if (dbConnection == null)
            {
                dbConnection = new DbConnection(this.dbConnectionString);
            }

            return dbConnection;
        }

        protected TContext CreateContext()
        {
            return this.CreateContext(this.databaseInitializer);
        }

        protected TContext CreateContext(IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            var args = new List<object>();
            if (!string.IsNullOrEmpty(this.dbConnectionString))
            {
                args.Add(this.dbConnectionString);
            }
            else
            {
                var dbConn = this.EnsureDbConnection(this.dbConnection);
                args.Add(dbConn);
            }

            if (databaseInitializer == null)
            {
                databaseInitializer = this.EnsureDatabaseInitializer(this.databaseInitializer);
            }
            args.Add(databaseInitializer);

            if (this.Log != null)
            {
                args.Add(this.Log);
            }

            return this.CreateContext(args.ToArray());
        }

        protected TContext CreateContext(params object[] args)
        {
            var contextType = typeof(TContext);
            var context = CreateContextInstance(contextType, args);

            this.contextInstances.Add(context);
            return context;
        }

        private static TContext CreateContextInstance(Type contextType, params object[] args)
        {
            var contextCtor = contextType.GetMatchingConstructor(args);
            return (TContext)contextCtor.Invoke();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.DeleteDatabaseOnDispose)
                {
                    if (this.contextInstances.Any())
                    {
                        // Drop & dispose all created context instances (if they've not already been disposed during test execution)
                        var dropped = false;
                        foreach (var context in this.contextInstances.Where(c => !c.IsDisposed))
                        {
                            if (dropped == false)
                            {
                                context.DropDatabase();
                                dropped = true;
                            }
                            context.Dispose();
                        }

                        // If all contexts are disposed, create a new context to drop the database
                        if (dropped == false)
                        {
                            using (var context = this.CreateContext(new CreateDatabaseIfNotExists<TContext>()))
                            {
                                context.DropDatabase();
                            }
                        }
                    }
                }

                this.contextInstances.Clear();
            }

            this.disposed = true;
        }
    }
}