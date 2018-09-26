using EntityFramework.Toolkit.EF6;
using EntityFramework.Toolkit.EF6.Contracts;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Contracts.Repository
{
    public interface IEmployeeReadOnlyRepository : IReadOnlyRepository<Employee>
    {
    }
}