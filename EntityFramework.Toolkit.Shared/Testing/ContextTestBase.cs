using System;
using System.Data.Entity;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

namespace EntityFramework.Toolkit.Testing
{
    public abstract class ContextTestBase<TContext> : IDisposable where TContext : DbContext
    {
        private readonly IDbConnection dbConnection;
        private readonly IDatabaseInitializer<TContext> databaseInitializer;

        protected TContext Context { get; set; }

        protected bool DeleteDatabaseOnDispose { get; set; }

        protected ContextTestBase(IDbConnection dbConnection, bool deleteDatabaseOnDispose)
          : this(dbConnection: dbConnection,
                initializeDatabase: true,
                databaseInitializer: null,
                deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(IDbConnection dbConnection, IDatabaseInitializer<TContext> initializer)
            : this(dbConnection: dbConnection,
                  initializeDatabase: true,
                  databaseInitializer: initializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextTestBase{TContext}"/> class.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> which is used to connect to the database.</param>
        /// <param name="initializeDatabase">Determines if the database needs to be initialized at construction time.  (Default is true).</param>
        /// <param name="databaseInitializer">The <see cref="IDatabaseInitializer{TContext}"/> which is used initialize the database. (Default is <see cref="DropCreateDatabaseAlways{TContext}"/>).</param>
        /// <param name="deleteDatabaseOnDispose">Determines if the database needs to be deleted on dispose. (Default is true).</param>
        protected ContextTestBase(IDbConnection dbConnection, bool initializeDatabase = true, IDatabaseInitializer<TContext> databaseInitializer = null, bool deleteDatabaseOnDispose = true)
        {
            this.DeleteDatabaseOnDispose = deleteDatabaseOnDispose;
            this.dbConnection = dbConnection;
            this.databaseInitializer = databaseInitializer ?? new DropCreateDatabaseAlways<TContext>();

            if (initializeDatabase)
            {
                this.InitializeDatabase(databaseInitializer);
            }
        }

        /// <summary>
        /// Returns the default database initializer (given by ctor) if <paramref name="databaseInitializer"/> is null.
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
        /// Returns the default db connection (given by ctor) if <paramref name="dbConnection"/> is null.
        /// </summary>
        private IDbConnection EnsureDbConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                dbConnection = this.dbConnection;
            }

            return dbConnection;
        }

        protected void InitializeDatabase(IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            this.Context = this.CreateContext(this.dbConnection, databaseInitializer);

            if (databaseInitializer is DropCreateDatabaseAlways<TContext>)
            {
                // Only force initialization if the initializer is a DropCreateDatabaseAlways
                this.Context.Database.Initialize(force: true);
            }
        }

        protected TContext CreateContext(IDbConnection dbConnection = null, IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            databaseInitializer = this.EnsureDatabaseInitializer(databaseInitializer);
            dbConnection = this.EnsureDbConnection(dbConnection);

            var contextType = typeof(TContext);
            return (TContext)Activator.CreateInstance(contextType, dbConnection, databaseInitializer);
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
                if (this.Context != null)
                {
                    if (this.DeleteDatabaseOnDispose)
                    {
                        this.Context.Database.KillConnectionsToTheDatabase();
                        this.Context.Database.Delete();
                    }
                    this.Context.Dispose();
                    this.Context = null;
                }
            }
        }
    }
}