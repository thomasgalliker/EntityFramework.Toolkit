using System.Data.Entity;
using System.Data.Extensions.Extensions;

namespace System.Data.Extensions.Testing
{
    public abstract class ContextTestBase<T> : IDisposable where T : DbContext
    {
        private readonly IDbConnection dbConnection;

        protected T Context { get; set; }

        protected bool DeleteDatabaseOnDispose { get; set; }

        protected ContextTestBase(IDbConnection dbConnection, bool deleteDatabaseOnDispose)
          : this(dbConnection: dbConnection, 
                initializeDatabase: true,
                databaseInitializer: null,
                deleteDatabaseOnDispose: deleteDatabaseOnDispose)
        {
        }

        protected ContextTestBase(IDbConnection dbConnection, IDatabaseInitializer<T> initializer) 
            : this(dbConnection: dbConnection, 
                  initializeDatabase: true,
                  databaseInitializer: initializer)
        {
        }

        protected ContextTestBase(IDbConnection dbConnection, bool initializeDatabase = true, IDatabaseInitializer<T> databaseInitializer = null, bool deleteDatabaseOnDispose = true)
        {
            this.DeleteDatabaseOnDispose = deleteDatabaseOnDispose;
            this.dbConnection = dbConnection;

            if (initializeDatabase)
            {
               this.InitializeDatabase(databaseInitializer);
            }
        }

        protected void InitializeDatabase(IDatabaseInitializer<T> databaseInitializer = null)
        {
            var contextType = typeof(T);
            databaseInitializer = databaseInitializer ?? new DropCreateDatabaseAlways<T>();

            this.Context = (T)Activator.CreateInstance(contextType, this.dbConnection, databaseInitializer);

            if (databaseInitializer is DropCreateDatabaseAlways<T>)
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