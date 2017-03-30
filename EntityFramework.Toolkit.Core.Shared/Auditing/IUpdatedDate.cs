using System;

namespace EntityFramework.Toolkit.Auditing
{
    public interface IUpdatedDate
    {
        DateTime? UpdatedDate { get; set; }
    }
}