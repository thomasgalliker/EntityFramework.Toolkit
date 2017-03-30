using System;
using System.Diagnostics;

using EntityFramework.Toolkit.Auditing;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Person: Id={Id}, FirstName={FirstName}, LastName={LastName}")]
    public class Person : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime Birthdate { get; set; }

        public string CountryId { get; set; }

        public virtual Country Country { get; set; }

        public byte[] RowVersion { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}