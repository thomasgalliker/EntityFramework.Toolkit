using System;
using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Student: Id={Id}, FirstName={FirstName}, LastName={LastName}")]
    public class Student : Person
    {
        public DateTime EnrollmentDate { get; set; }
    }
}