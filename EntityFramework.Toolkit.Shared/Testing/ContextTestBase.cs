using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit.Testing
{
    public abstract class ContextTestBase<TContext> : IDisposable
        where TContext : DbContextBase<TContext>
    {
        private readonly ICollection<TContext> contextInstances = new List<TContext>();
        private readonly IDbConnection dbConnection;
        private readonly IDatabaseInitializer<TContext> databaseInitializer;
        private readonly string dbConnectionString;
        private bool disposed;

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

        protected ContextTestBase(Func<string> dbConnectionString, Action<string> log = null, IDatabaseInitializer<TContext> databaseInitializer = null, bool deleteDatabaseOnDispose = true)
        {
            this.dbConnectionString = dbConnectionString();
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

            if (dbConnection == null)
            {
                dbConnection = new DbConnection(this.dbConnectionString);
            }

            return dbConnection;
        }

        protected TContext CreateContext()
        {
            return this.CreateContext(this.dbConnection, this.databaseInitializer);
        }

        protected TContext CreateContext(IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            return this.CreateContext(this.dbConnection, databaseInitializer);
        }

        protected TContext CreateContext(IDbConnection dbConnection = null, IDatabaseInitializer<TContext> databaseInitializer = null)
        {
            databaseInitializer = this.EnsureDatabaseInitializer(databaseInitializer);
            dbConnection = this.EnsureDbConnection(dbConnection);

            if (!string.IsNullOrEmpty(this.dbConnectionString))
            {
                return this.CreateContext(this.dbConnectionString);
            }

            return this.CreateContext(dbConnection, databaseInitializer, this.Log);
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
            var argTypes = args.Where(d => d != null).Select(d => d.GetType()).ToArray();
            var contextCtor = contextType.GetConstructor(argTypes);
            if (contextCtor == null)
            {
                var sb = new StringBuilder();
                var definedParameters = string.Join(", ", argTypes.Select(p => p.GetFormattedName()));
                sb.AppendLine(
                    definedParameters.Length == 0
                        ? $"{contextType.Name} does not have a constructor with no parameters."
                        : $"{contextType.Name} does not have a constructor with parameter{(definedParameters.Length > 1 ? "s" : "")} ({definedParameters}).");

                var constructors = contextType.GetConstructors();
                if (constructors.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("Use one of the following constructors:");
                    foreach (var constructor in constructors)
                    {
                        var parameters = $"{string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))}";
                        sb.AppendLine($"{contextType.Name}({parameters})");
                    }
                }

                var exceptionMessage = sb.ToString();
                throw new InvalidOperationException(exceptionMessage);
            }

            return (TContext)contextCtor.Invoke(args);
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