using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

namespace EntityFramework.Toolkit
{
    /// <summary>
    /// Implementation of a generic repository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly DbSet<T> DbSet;
        private readonly IDbContext context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
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

        public virtual bool Any(object id)
        {
            return this.DbSet.Find(id) != null;
        }

        /// <inheritdoc />
        public T FindById(params object[] ids)
        {
            return this.DbSet.Find(ids);
        }

        /// <inheritdoc />
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = this.DbSet.Where(predicate).AsEnumerable();
            return query;
        }

        /// <inheritdoc />
        public virtual T Add(T entity)
        {
            return this.DbSet.Add(entity);
        }

        /// <inheritdoc />
        public virtual IEnumerable<T> AddRange(IEnumerable<T> entity)
        {
            return this.DbSet.AddRange(entity);
        }

        /// <inheritdoc />
        public virtual T AddOrUpdate(T entity)
        {
            return ((DbContext)this.context).AddOrUpdate(entity);
        }

        public virtual void Update(T entity)
        {
            this.context.Edit(entity);
        }

        /// <inheritdoc />
        public virtual T Remove(T entity)
        {
            return this.DbSet.Remove(entity);
        }

        /// <inheritdoc />
        public virtual IEnumerable<T> RemoveAll(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = this.DbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return this.DbSet.RemoveRange(query);
        }

        /// <inheritdoc />
        public virtual IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            return this.DbSet.RemoveRange(entities);
        }

        /// <inheritdoc />
        public virtual void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty) where TEntity : class where TProperty : class
        {
            this.context.LoadReferenced(entity, navigationProperty);
        }

        /// <inheritdoc />
        public virtual ChangeSet Save()
        {
            return this.context.SaveChanges();
        }
    }
}