using System;

namespace EntityFramework.Toolkit.EF6.Contracts.Auditing
{
    public interface IUpdatedDate
    {
        DateTime? UpdatedDate { get; set; }
    }
}