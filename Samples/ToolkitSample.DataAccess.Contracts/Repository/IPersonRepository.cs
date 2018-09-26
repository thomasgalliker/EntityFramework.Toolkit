using EntityFramework.Toolkit.EF6.Contracts;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Contracts.Repository
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
    }
}