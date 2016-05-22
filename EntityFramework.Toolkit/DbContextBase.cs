using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.Extensions.Extensions;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System.Data.Extensions
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

        protected DbContextBase(IDbConnection dbConnection, IDatabaseInitializer<TContext> databaseInitializer) : this()
        {
            this.Database.Connection.ConnectionString = dbConnection.ConnectionString;
            this.databaseInitializer = databaseInitializer;
            Database.SetInitializer(databaseInitializer);
        }

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
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException validationException)
            {
                string errorMessage = validationException.GetFormattedErrorMessage();
                throw new DbEntityValidationException(errorMessage, validationException);
            }
        }

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
    }
}