

using EntityFramework.Toolkit;

using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Contracts.Repository
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
    }
}