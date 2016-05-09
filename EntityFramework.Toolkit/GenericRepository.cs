using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Extensions
{
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly IDbSet<T> DbSet;
        private readonly IDbContext context;

        protected GenericRepository(IDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;

            this.DbSet = this.context.Set<T>();
        }

        public IContext Context
        {
            get
            {
                return this.context;
            }
        }

        public IQueryable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = this.DbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.DbSet.AsEnumerable<T>();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = this.DbSet.Where(predicate).AsEnumerable();
            return query;
        }

        public virtual T Add(T entity)
        {
            return this.DbSet.Add(entity);
        }

        public virtual T Delete(T entity)
        {
            return this.DbSet.Remove(entity);
        }

        public virtual void DeleteAll(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = this.DbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var entity in query)
            {
                this.context.Delete(entity);
            }
        }

        public virtual void Edit(T entity)
        {
           this.Context.Edit(entity);
        }

        public virtual void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class
        {
            this.Context.LoadReferenced(entity, navigationProperty);
        }

        public virtual void Save()
        {
            this.Context.SaveChanges();
        }
    }
}