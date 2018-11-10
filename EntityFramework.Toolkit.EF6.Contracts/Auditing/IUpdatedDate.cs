using System;

namespace EntityFramework.Toolkit.Core.Auditing
{
    public interface IUpdatedDate
    {
        DateTime? UpdatedDate { get; set; }
    }
}