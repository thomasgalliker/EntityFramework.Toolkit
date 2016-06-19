using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
#if !NET40
using System.Threading.Tasks;
#endif

using EntityFramework.Toolkit.Concurrency;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Exceptions;
using EntityFramework.Toolkit.Extensions;

namespace EntityFramework.Toolkit
{
    public abstract class DbContextBase<TContext> : DbContext, IDbContext where TContext : DbContext
    {
        private readonly IDatabaseInitializer<TContext> databaseInitializer;

        /// <summary>
        /// Empty constructor is used for 'update-database' command-line command.
        /// </summary>
        protected DbContextBase()
        {
            this.Database.Log = message => Debug.WriteLine(message);
        }

        //protected DbContextBase(IDbConnection dbConnection) : this(dbConnection, null)
        //{
        //}

        protected DbContextBase(IDbConnection dbConnection, IDatabaseInitializer<TContext> databaseInitializer) : this()
        {
            this.Database.Connection.ConnectionString = dbConnection.ConnectionString;
            this.Configuration.LazyLoadingEnabled = dbConnection.LazyLoadingEnabled;
            this.Configuration.ProxyCreationEnabled = dbConnection.ProxyCreationEnabled;
            this.Name = dbConnection.Name;
            this.databaseInitializer = databaseInitializer;
            Database.SetInitializer(databaseInitializer);
            this.ConcurrencyResolveStrategy = new RethrowConcurrencyResolveStrategy();
        }

        public string Name { get; private set; }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void ResetDatabase()
        {
            this.Database.KillConnectionsToTheDatabase();

            // Set DropCreate initializer and force initialize
            Database.SetInitializer(new DropCreateDatabaseAlways<TContext>());
            this.Database.Initialize(force: true);

            // Restore original initializer
            Database.SetInitializer(this.databaseInitializer);
            this.Database.Initialize(force: true);
        }
        
        public void Edit<TEntity>(TEntity entity) where TEntity : class
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this.Entry(entity).State = EntityState.Deleted;
        }

        public void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class
        {
            this.Entry(entity).Reference(navigationProperty).Load();
        }

        public override int SaveChanges()
        {
            //var changeSet = this.GetChangeSet();
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException validationException)
            {
                string errorMessage = validationException.GetFormattedErrorMessage();
                throw new DbEntityValidationException(errorMessage, validationException);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                // Get the current entity values and the values in the database 
                // as instances of the entity type 
                var entry = concurrencyException.Entries.Single();
                var databaseValues = entry.GetDatabaseValues();
                if (databaseValues == null)
                {
                    throw new UpdateConcurrencyException("Failed to update an entity which which has previsouly been deleted.", concurrencyException);
                }

                var databaseValuesAsObject = databaseValues.ToObject();
                var conflictingEntity = entry.Entity;

                // Have the user choose what the resolved values should be 
                var resolvedValuesAsObject = this.ConcurrencyResolveStrategy.ResolveConcurrencyException(conflictingEntity, databaseValuesAsObject);
                if (resolvedValuesAsObject == null || this.ConcurrencyResolveStrategy is RethrowConcurrencyResolveStrategy)
                {
                    if (this.ConcurrencyResolveStrategy is RethrowConcurrencyResolveStrategy)
                    {
                        throw;
                    }
                }

                // Update the original values with the database values and 
                // the current values with whatever the user choose. 
                entry.OriginalValues.SetValues(databaseValues);
                entry.CurrentValues.SetValues(resolvedValuesAsObject);

                //TODO: Handle number of max retries

                return this.SaveChanges();

                //////assume just one
                ////var dbEntityEntry = concurrencyException.Entries.First();
                //////store wins
                ////dbEntityEntry.Reload();
                //////OR client wins
                ////var dbPropertyValues = dbEntityEntry.GetDatabaseValues();
                ////dbEntityEntry.OriginalValues.SetValues(dbPropertyValues); //orig = db

                ////throw;
            }
            catch (DbUpdateException updateException)
            {
                //often in innerException
                if (updateException.InnerException != null)
                    Debug.WriteLine(updateException.InnerException.Message);
                //which exceptions does it relate to
                foreach (var entry in updateException.Entries)
                {
                    Debug.WriteLine(entry.Entity);
                }

                throw;
            }
        }

        public virtual object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return null;
        }

        public IConcurrencyResolveStrategy ConcurrencyResolveStrategy { get; set; }

#if !NET40
        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException validationException)
            {
                string errorMessage = validationException.GetFormattedErrorMessage();
                throw new DbEntityValidationException(errorMessage, validationException);
            }
        }
#endif

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }


        /// <summary>
        /// Determins the changes that are transfered to the persistence layer.
        /// </summary>
        /// <returns>ChangeSet.</returns>
        private ChangeSet GetChangeSet()
        {
            var updatedEntries = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified && e.Entity != null);
            IList<IChange> updateChanges = new List<IChange>();

            foreach (var dbEntityEntry in updatedEntries)
            {
                IList<PropertyChangeInfo> changes = new List<PropertyChangeInfo>();
                foreach (var propertyName in dbEntityEntry.CurrentValues.PropertyNames)
                {
                    if (dbEntityEntry.Property(propertyName).IsModified)
                    {
                        changes.Add(new PropertyChangeInfo(propertyName, dbEntityEntry.Property(propertyName).CurrentValue));
                    }
                }
                updateChanges.Add(Change.CreateUpdateChange(dbEntityEntry.Entity, changes));
            }

            var addChanges = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity != null).Select(e => Change.CreateAddedChange(e.Entity));
            var deleteChanges = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity != null).Select(n => Change.CreateDeleteChange(n.Entity));

            List<IChange> result = new List<IChange>(addChanges);
            result.AddRange(deleteChanges);
            result.AddRange(updateChanges);

            return new ChangeSet(result);
        }

        /// <summary>
        /// Created the <see cref="DbConnection" /> based on the provider.
        /// </summary>
        /// <param name="providerName">Name of the data provider.</param>
        /// <param name="providerConnectionString">Name of the connection string.</param>
        /// <returns>Returns the connection if providerName and providerConnectionString are set. Returns null otherwise.</returns>
        ////protected static System.Data.Common.DbConnection GetConnection(string providerName, string providerConnectionString)
        ////{
        ////    System.Data.Common.DbConnection connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
        ////    connection.ConnectionString = providerConnectionString;

        ////    return connection;
        ////}

        /// <summary>
        /// Closes the database connection.
        /// The connection might remain open, until a (distributed) transaction has been commited or rolled back.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    // Close the connection because the context gets disposed
        //    this.Database.Connection.Close();
        //    base.Dispose(disposing);
        //}
    }
}