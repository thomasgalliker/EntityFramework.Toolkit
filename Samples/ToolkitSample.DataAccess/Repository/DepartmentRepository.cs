using EntityFramework.Toolkit;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Repository
{
    /// <summary>
    /// The DepartmentRepository should only be visibile withing the DAL layer.
    /// Consumers have to consume the IDepartmentRepository interface.
    /// </summary>
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IEmployeeContext context)
            : base(context)
        {
        }
    }
}
