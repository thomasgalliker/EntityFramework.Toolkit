using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ToolkitSample.DataAccess.Stubs
{
    /// <summary>
    /// This is an in-memory, List backed implementation of
    /// Entity Framework's System.Data.Entity.IDbSet to use
    /// for testing.
    /// </summary>
    /// <typeparam name="T">The type of entity to store.</typeparam>
    public class FakeDbSet<T> : DbSet<T> where T : class
    {
        private readonly List<T> _data;

        public FakeDbSet()
        {
            this._data = new List<T>();
        }

        public FakeDbSet(params T[] entities)
        {
            this._data = new List<T>(entities);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._data.GetEnumerator();
        }

        public Expression Expression
        {
            get { return Expression.Constant(this._data.AsQueryable()); }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return this._data.AsQueryable().Provider; }
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Wouldn't you rather use Linq .SingleOrDefault()?");
        }

        public T Add(T entity)
        {
            this._data.Add(entity);
            return entity;
        }

        public T Remove(T entity)
        {
            this._data.Remove(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            this._data.Add(entity);
            return entity;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public ObservableCollection<T> Local
        {
            get { return new ObservableCollection<T>(this._data); }
        }
    }
}
