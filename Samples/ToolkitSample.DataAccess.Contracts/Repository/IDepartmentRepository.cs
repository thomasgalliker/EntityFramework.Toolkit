using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Contracts.Repository
{
    /// <summary>
    /// This interface abstracts the DepartmentRepository implementation.
    /// In a real-world scenario, this interface could be placed inside a dedicated DAL.Contracts assembly.
    /// Extend it with your custom CRUD queries as needed.
    /// </summary>
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
    }
}