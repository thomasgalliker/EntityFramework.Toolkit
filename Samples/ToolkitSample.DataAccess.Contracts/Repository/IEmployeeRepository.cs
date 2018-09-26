using EntityFramework.Toolkit.EF6.Contracts;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Contracts.Repository
{
    /// <summary>
    /// This interface abstracts the EmployeeRepository implementation.
    /// In a real-world scenario, this interface could be placed inside a dedicated DAL.Contracts assembly.
    /// Extend it with your custom CRUD queries as needed.
    /// </summary>
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
    }
}