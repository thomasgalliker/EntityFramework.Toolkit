
using System.Data.Entity;
using System.Data.Extensions;

using Autofac;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.DataAccess.Repository;
using ToolkitSample.DataAccess.Seed;

namespace ToolkitSample.DataAccess.Modularity
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DepartmentDataSeed>().As<IDataSeed>().SingleInstance(); // TODO: Find a better way to register data seeds

            builder.RegisterType<EmployeeContextDbConnection>().As<IDbConnection>().SingleInstance();
            builder.RegisterType<EmployeeContextDatabaseInitializer>().As<IDatabaseInitializer<EmployeeContext>>().SingleInstance();
            builder.RegisterType<EmployeeContext>().As<IEmployeeContext>().InstancePerDependency();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerDependency();
        }
    }
}