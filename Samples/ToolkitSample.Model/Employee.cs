using System;
using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Employee: Id={Id}, FirstName={FirstName}, LastName={LastName}, Department={Department?.Name}")]
    public class Employee : Person
    {
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public DateTime? EmployementDate { get; set; }
    }
}