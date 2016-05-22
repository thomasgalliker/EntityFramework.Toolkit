using System.Data.Entity;
using System.Data.Extensions.Extensions;

namespace System.Data.Extensions.Testing
{
    public abstract class ContextTestBase<TContext> : IDisposable where TContext : DbContext
    {
        private readonly IDbConnection dbConnection;

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

            if (initializeDatabase)
            {
               this.InitializeDatabase(databaseInitializer);
            }
        }

        protected void InitializeDatabase(IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            var contextType = typeof(TContext);
            databaseInitializer = databaseInitializer ?? new DropCreateDatabaseAlways<TContext>();

            this.Context = (TContext)Activator.CreateInstance(contextType, this.dbConnection, databaseInitializer);

            if (databaseInitializer is DropCreateDatabaseAlways<TContext>)
            {
                // Only force initialization if the initializer is a DropCreateDatabaseAlways
                this.Context.Database.Initialize(force: true);
            }
        }
        
        public void Dispose()
        {
            if (this.Context != null)
            {
                if (this.DeleteDatabaseOnDispose)
                {
                    this.Context.Database.KillConnectionsToTheDatabase();
                    this.Context.Database.Delete();
                }

                this.Context.Dispose();
            }
        }
    }
}