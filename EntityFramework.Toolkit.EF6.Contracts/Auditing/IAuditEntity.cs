using System;

namespace EntityFramework.Toolkit.EF6.Contracts.Auditing
{
    /// <summary>
    ///     Any class used for auditing an entity must implement this inteface.
    /// </summary>
    public interface IAuditEntity<TKey> : IAuditEntity
    {
        /// <summary>
        /// The primary key of an audit entry.
        /// </summary>
        TKey AuditId { get; set; }
    }

    public interface IAuditEntity
    {
        /// <summary>
        ///     Gets or sets the DateTime this audit entity was created.
        /// </summary>
        /// <remarks>
        ///     Will be automatically set by AuditDbContext on SaveChanges.
        /// </remarks>
        DateTime AuditDate { get; set; }

        /// <summary>
        ///     Gets or sets the user who updated the entity
        /// </summary>
        /// <remarks>
        ///     Will be automatically set by AuditDbContext on SaveChanges.
        /// </remarks>
        string AuditUser { get; set; }

        /// <summary>
        ///     Gets or sets the type of audit.
        /// </summary>
        /// <remarks>
        ///     Will be automatically set by AuditDbContext on SaveChanges.
        /// </remarks>
        AuditEntityState AuditType { get; set; }
    }
}