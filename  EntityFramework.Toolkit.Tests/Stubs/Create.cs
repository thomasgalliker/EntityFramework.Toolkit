using System;
using System.Collections.Generic;

using ToolkitSample.DataAccess.Model;

namespace EntityFramework.Toolkit.Tests.Stubs
{
    public static class CreateEntity
    {
        public static readonly Employee Employee1 = new Employee { FirstName = "Thomas", LastName = "Galliker", Birthdate = new DateTime(1986, 07, 11) };
        public static readonly Employee Employee2 = new Employee { FirstName = "Fritz", LastName = "Müller", Birthdate = new DateTime(1990, 01, 01) };
        public static readonly Employee Employee3 = new Employee { FirstName = "Lorem", LastName = "Ipsum", Birthdate = new DateTime(2000, 12, 31) };

        public static readonly Department Department1 = new Department { Name = "Human Resources", Leader = Employee1, Employees = new List<Employee> { Employee2, Employee3 } };
        public static readonly Department Department2 = new Department { Name = "Development", Leader = Employee2, Employees = new List<Employee> { Employee1, Employee3 } };
    }
}
