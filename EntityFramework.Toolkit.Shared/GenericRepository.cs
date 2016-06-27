using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
{
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly DbSet<T> DbSet;
        private readonly IDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        protected GenericRepository(IDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;

            this.DbSet = this.context.Set<T>();
        }

        /// <inheritdoc />
        public IContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual IEnumerable<T> GetAll()
        {
            return this.DbSet.AsEnumerable<T>();
        }

        /// <inheritdoc />
        public T FindById(params object[] ids)
        {
            return this.DbSet.Find(ids);
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

        public virtual IEnumerable<T> AddRange(IEnumerable<T> entity)
        {
            return this.DbSet.AddRange(entity);
        }

        public virtual T Remove(T entity)
        {
            return this.DbSet.Remove(entity);
        }

        public virtual IEnumerable<T> RemoveAll(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = this.DbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return this.DbSet.RemoveRange(query);
        }

        public virtual IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            return this.DbSet.RemoveRange(entities);
        }

        public virtual void Edit(T entity)
        {
           this.Context.Edit(entity);
        }

        /// <inheritdoc />
        public virtual void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class
        {
            this.Context.LoadReferenced(entity, navigationProperty);
        }

        /// <inheritdoc />
        public virtual ChangeSet Save()
        {
            return this.Context.SaveChanges();
        }
    }
}