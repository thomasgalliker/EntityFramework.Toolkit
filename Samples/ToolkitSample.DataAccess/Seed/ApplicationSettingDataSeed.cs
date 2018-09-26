using System;
using System.Linq.Expressions;

using EntityFramework.Toolkit.EF6;

using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Seed
{
    internal sealed class ApplicationSettingDataSeed : DataSeedBase<ApplicationSetting>
    {
        public override Expression<Func<ApplicationSetting, object>> AddOrUpdateExpression
        {
            get
            {
                return applicationSetting => applicationSetting.Id;
            }
        }

        public override ApplicationSetting[] GetAll()
        {
            return new[]
            {
                new ApplicationSetting { Path = "/../../TestForSeed" }
            };
        }
    }
}
