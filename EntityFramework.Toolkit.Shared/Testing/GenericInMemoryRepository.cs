using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit.Testing
{
    public class GenericInMemoryRepository<T> : IGenericRepository<T>
    {
        private readonly List<T> store;

        public GenericInMemoryRepository()
        {
            this.store = new List<T>();
        }
        
        public void Save(T item)
        {
            store.RemoveAll(WhereIdMatches(item.Id));
            store.Add(item);
        }

        public void Delete(IComparable id)
        {
            store.RemoveAll(WhereIdMatches(id));
        }

        public T FindById(IComparable id)
        {
            return store.Find(WhereIdMatches(id));
        }

        private static Predicate<T> WhereIdMatches(IComparable id)
        {
            return i => i.Id.Equals(id);
        }

        public IContext Context { get; }

        public IQueryable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return this.store;
        }

        public T FindById(params object[] ids)
        {
            return store.Find(WhereIdMatches(ids));
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return this.store.Find(x => x.)
        }

        public T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }

        public T Remove(T entity)
        {
            this.store.Remove(entity);
            return entity;
        }

        public IEnumerable<T> RemoveAll(Expression<Func<T, bool>> predicate = null)
        {
            this.store.RemoveAll(this.store.Where(predicate));
        }

        public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Edit(T entity)
        {
        }

        public void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty) where TEntity : class where TProperty : class
        {
        }

        public ChangeSet Save()
        {
            return ChangeSet.Empty;
        }
    }
}
