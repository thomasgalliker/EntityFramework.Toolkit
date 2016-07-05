using System.Collections.Generic;
using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
{
    /// <summary>
    ///     Interface for changed objects and object properties.
    /// </summary>
    public interface IChange
    {
        /// <summary>
        ///     Gets the changed object.
        /// </summary>
        /// <value>
        ///     See <see cref="object" />.
        /// </value>
        object ChangedEntity { get; }

        /// <summary>
        ///     Gets the names of the properties that have been changed.
        /// </summary>
        /// <value>
        ///     The names of the properties that have been changed.
        /// </value>
        IEnumerable<PropertyChangeInfo> ChangedProperties { get; }

        /// <summary>
        ///     Gets the state of the changed object.
        /// </summary>
        /// <value>
        ///     See <see cref="EntityState" />.
        /// </value>
        ChangeState State { get; }
    }
}