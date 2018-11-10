using EntityFramework.Toolkit;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;

namespace ToolkitSample.DataAccess.Repository
{
    /// <summary>
    /// The EmployeeRepository should only be visible withing the DAL layer.
    /// Consumers have to consume the IEmployeeRepository interface.
    /// </summary>
    public class EmployeeRepository : GenericRepository<Model.Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IEmployeeContext context)
            : base(context)
        {
        }
    }
}
