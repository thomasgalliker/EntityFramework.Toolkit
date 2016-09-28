using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using ToolkitSample.DataAccess.Stubs;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    /// <summary>
    /// An in-memory implementation of a DbContextBase
    /// </summary>
    public sealed class SampleContext : ISampleContext
    {
        /// <summary>
        /// Initializes a new instance of the FakeEmployeeContext class.
        /// The context contains empty initial data.
        /// </summary>
        public SampleContext()
        {
            this.Employees = new FakeDbSet<Model.Employee>();
            this.Departments = new FakeDbSet<Department>();
        }

        /// <summary>
        /// Initializes a new instance of the FakeEmployeeContext class.
        /// The context contains the supplied initial data.
        /// </summary>
        /// <param name="employees">Employees to include in the context</param>
        /// <param name="departments">Departments to include in the context</param>
        ////public SampleContext(IEnumerable<Employee> employees, IEnumerable<Department> departments)
        ////{
        ////    if (employees == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(employees));
        ////    }

        ////    if (departments == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(departments));
        ////    }

        ////    this.Employees = new FakeDbSet<Employee>(employees);
        ////    this.Departments = new FakeDbSet<Department>(departments);
        ////}

        public event EventHandler<EventArgs> SaveCalled;

        public event EventHandler<EventArgs> DisposeCalled;

        public IDbSet<Model.Employee> Employees { get; private set; }

        public IDbSet<Department> Departments { get; private set; }

        /// <summary>
        /// Save all pending changes in this context
        /// </summary>
        public void Save()
        {
            this.OnSaveCalled(EventArgs.Empty);
        }

        public void Dispose()
        {
            this.OnDisposeCalled(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the SaveCalled event
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        private void OnSaveCalled(EventArgs e)
        {
            var handler = this.SaveCalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DisposeCalled event
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        private void OnDisposeCalled(EventArgs e)
        {
            var handler = this.DisposeCalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ResetDatabase()
        {
            throw new NotImplementedException();
        }

        public void DropDatabase()
        {
            throw new NotImplementedException();
        }

        TEntity IContext.Edit<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Edit<TEntity>(TEntity originalEntity, TEntity updateEntity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity Delete<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty) where TEntity : class where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.Factory.StartNew(() => 99);
        }

        public string Name
        {
            get
            {
                return "Sample Context";
            }
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return new FakeDbSet<TEntity>();
        }

        ChangeSet IContext.SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
