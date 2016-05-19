using System.Data.Extensions;

namespace ToolkitSample.DataAccess.Repository
{
    /// <summary>
    /// The EmployeeRepository should only be visibile withing the DAL layer.
    /// Consumers have to consume the IEmployeeRepository interface.
    /// </summary>
    public class EmployeeRepository : GenericRepository<Model.Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
