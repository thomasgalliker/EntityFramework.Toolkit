using System;

namespace EntityFramework.Toolkit.Core.Auditing
{
    /// <summary>
    ///     Describes the state of an audit entity.
    /// </summary>
    [Flags]
    public enum AuditEntityState
    {
        Added = 4,
        Deleted = 8,
        Modified = 16
    }
}