using System;
using System.Data.Extensions;
using System.Linq.Expressions;

using ToolkitSample.DataAccess.Model;

namespace ToolkitSample.DataAccess.Seed
{
    internal sealed class DepartmentDataSeed : DataSeedBase<Department>
    {
        public override Expression<Func<Department, object>> AddOrUpdateExpression
        {
            get
            {
                return department => department.Name;
            }
        }

        public override Department[] GetAll()
        {
            return new[]
            {
                new Department {Name = "Administration"},
                new Department {Name = "Human Resources"}
            };
        }
    }
}
