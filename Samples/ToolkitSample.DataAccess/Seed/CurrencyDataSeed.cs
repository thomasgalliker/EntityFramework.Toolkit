using System.Data.Extensions;

namespace ToolkitSample.DataAccess.Seed
{
    internal sealed class CurrencyDataSeed : DataSeedBase<Currency>
    {
        public override Currency[] GetAll()
        {
            return new[]
            {
                new Currency {Name = "CHF"},
                new Currency {Name = "EUR"}
            };
        }
    }
}
