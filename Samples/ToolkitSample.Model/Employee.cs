using System;
using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Employee: Id={Id}, FirstName={FirstName}, LastName={LastName}")]
    public class Employee
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public string CountryId { get; set; }

        public virtual Country Country { get; set; }

        public byte[] RowVersion { get; set; }
    }
}