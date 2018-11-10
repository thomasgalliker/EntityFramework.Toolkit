using System;
using System.Data.Entity;
using EntityFramework.Toolkit.Core.Auditing;

namespace EntityFramework.Toolkit.Auditing.Extensions
{
    public static class AuditEntityStateExtensions
    {
        public static AuditEntityState ToAuditEntityState(this EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Added:
                    return AuditEntityState.Added;
                case EntityState.Modified:
                    return AuditEntityState.Modified;
                case EntityState.Deleted:
                    return AuditEntityState.Deleted;

                default:
                    throw new NotSupportedException($"EntityState.{entityState} is not supported.");
            }
        }
    }
}