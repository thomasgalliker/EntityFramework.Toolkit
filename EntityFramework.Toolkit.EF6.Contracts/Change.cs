using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EntityFramework.Toolkit.EF6.Contracts
{
    [DebuggerDisplay("ChangedEntity='{ChangedEntity}', State={State}", Type = "Change")]
    public class Change : IChange
    {
        private Change(object changedEntity, ChangeState state)
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
        ///     Thrown if the specified <paramref name="changedEntity" /> is null.
        /// </exception>
        /// .
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the specified <paramref name="changedProperties" /> is null.
        /// </exception>
        private Change(object changedEntity, ChangeState state, IEnumerable<PropertyChangeInfo> changedProperties)
        {
            if(changedEntity == null)
            {
                throw new ArgumentNullException(nameof(changedEntity));
            }

            if (state == ChangeState.Modified && changedProperties == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(changedProperties)} must be defined if ChangeState is Modified.", nameof(changedProperties));
            }

            this.ChangedEntity = changedEntity;
            this.ChangedProperties = changedProperties;
            this.State = state;
        }

        public object ChangedEntity { get; private set; }

        public IEnumerable<PropertyChangeInfo> ChangedProperties { get; private set; }

        public ChangeState State { get; private set; }

        public static IChange CreateUpdateChange(object changedEntity, IEnumerable<PropertyChangeInfo> changedProperties)
        {
            return new Change(changedEntity, ChangeState.Modified, changedProperties);
        }

        public static IChange CreateDeleteChange(object changedEntity)
        {
            return new Change(changedEntity, ChangeState.Deleted);
        }

        public static IChange CreateAddedChange(object changedEntity)
        {
            return new Change(changedEntity, ChangeState.Added);
        }
    }
}