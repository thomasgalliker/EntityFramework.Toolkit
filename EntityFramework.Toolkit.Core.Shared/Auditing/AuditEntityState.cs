using System;

namespace EntityFramework.Toolkit.Auditing
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