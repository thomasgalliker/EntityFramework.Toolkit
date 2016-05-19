using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Extensions
{
    public abstract class DbContextBase<TContext> : DbContext, IDbContext where TContext : DbContext
    {
        /// <summary>
        /// Empty constructor is used for 'update-database' command-line command.
        /// </summary>
        public DbContextBase()
        {
        }

        public DbContextBase(string connectionString, IDatabaseInitializer<TContext> databaseInitializer)
        {
            this.Database.Log = this.Log;
            this.Database.Connection.ConnectionString = connectionString;
            Database.SetInitializer(databaseInitializer);
        }

        protected virtual void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
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
            this.Log(string.Format("Calling {0}.SaveChanges()", typeof(TContext).Name));

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException validationException)
            {
                var stringBuilder = new StringBuilder();

                // In case something goes wrong during entity validation
                // we trace the affected properties with its problems to the console and rethrow the exception
                foreach (var result in validationException.EntityValidationErrors)
                {
                    stringBuilder.AppendLine(
                        string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            result.Entry.Entity.GetType().Name,
                            result.Entry.State));

                    foreach (var ve in result.ValidationErrors)
                    {
                        stringBuilder.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            result.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                    stringBuilder.AppendLine();
                }

                string errorMessage = stringBuilder.ToString();
                this.Log(errorMessage);
                throw;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public interface ILoggable
    {
        void Log(string message);
    }
}