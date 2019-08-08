using EntityFramework.Toolkit;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Repository
{
    /// <summary>
    /// The PersonRepository should only be visibile withing the DAL layer.
    /// Consumers have to consume the IPersonRepository interface.
    /// </summary>
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(IEmployeeContext context)
            : base(context)
        {
        }
    }
}
