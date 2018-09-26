using System;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.EF6.Contracts
{
    public interface IDataSeed
    {
        /// <summary>
        /// Gets the expression which checks if there are existing seed entries.
        /// Depending on the result, the seed entry is added or updated.
        /// </summary>
        Expression<Func<object, object>> GetAddOrUpdateExpression();

        /// <summary>
        /// The type of the entity for which this seed is used.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// The seed data as a list of objects.
        /// </summary>
        object[] GetAllObjects();
    }
}
