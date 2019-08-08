
using EntityFramework.Toolkit;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Repository
{
    /// <summary>
    /// The EmployeeReadOnlyRepository should only be visibile withing the DAL layer.
    /// Consumers have to consume the IEmployeeReadOnlyRepository interface.
    /// </summary>
    public class EmployeeReadOnlyRepository : GenericRepository<Employee>, IEmployeeReadOnlyRepository
    {
        public EmployeeReadOnlyRepository(IEmployeeContext context)
            : base(context)
        {
        }
    }
}
