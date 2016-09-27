using System;

using ToolkitSample.Model;

namespace EntityFramework.Toolkit.Tests.Stubs
{
    public static class Testdata
    {
        public static class Employees
        {
            public static Employee CreateEmployee1()
            {
                return new Employee { FirstName = "Thomas", LastName = "Galliker", Birthdate = new DateTime(1986, 07, 11) };
            }

            public static Employee CreateEmployee2()
            {
                return new Employee { FirstName = "Fritz", LastName = "Müller", Birthdate = new DateTime(1990, 01, 01) };
            }

            public static Employee CreateEmployee3()
            {
                return new Employee { FirstName = "Lorem", LastName = "Ipsum", Birthdate = new DateTime(2000, 12, 31) };
            }
        }

        public static class Departments
        {
            public static Department CreateDepartmentHumanResources()
            {
                return new Department
                {
                    Name = "Human Resources",
                };
            }
        }
    }
}
