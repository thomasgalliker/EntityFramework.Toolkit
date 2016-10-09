using System;
using System.Linq.Expressions;

using EntityFramework.Toolkit;

using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Seed
{
    internal sealed class CountryDataSeed : DataSeedBase<Country>
    {
        public override Expression<Func<Country, object>> AddOrUpdateExpression
        {
            get
            {
                return applicationSetting => applicationSetting.Id;
            }
        }

        public override Country[] GetAll()
        {
            return new []
            {
                new Country { Id = "CH", Name = "Switzerland" },
                new Country { Id = "DE", Name = "Germany" },
                new Country { Id = "AT", Name = "Austria" },
            };
        }
    }
}
