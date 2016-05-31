using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;

namespace System.Data.Extensions
{
    [DebuggerDisplay("ChangedEntity='{ChangedEntity}', State={State}", Type = "Change")]
    public class Change : IChange
    {
        private Change(object changedEntity, EntityState state)
            : this(changedEntity, state, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Change" /> class.
        /// </summary>
        /// <param name="changedEntity">
        ///     The changed object.
        /// </param>
        /// <param name="changedProperties">
        ///     The List of the properties that have been changed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the specified <paramref name="changedEntity" /> is NULL.
        /// </exception>
        /// .
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the specified <paramref name="changedProperties" /> is NULL.
        /// </exception>
        private Change(object changedEntity, EntityState state, IEnumerable<PropertyChangeInfo> changedProperties)
        {
            this.ChangedEntity = changedEntity;
            this.ChangedProperties = changedProperties;
            this.State = state;
        }

        public object ChangedEntity { get; private set; }

        public IEnumerable<PropertyChangeInfo> ChangedProperties { get; private set; }

        public EntityState State { get; private set; }

        public static IChange CreateUpdateChange(object changedEntity, IEnumerable<PropertyChangeInfo> changedProperties)
        {
            return new Change(changedEntity, EntityState.Modified, changedProperties);
        }

        public static IChange CreateDeleteChange(object changedEntity)
        {
            return new Change(changedEntity, EntityState.Deleted);
        }

        public static IChange CreateAddedChange(object changedEntity)
        {
            return new Change(changedEntity, EntityState.Added);
        }
    }
}