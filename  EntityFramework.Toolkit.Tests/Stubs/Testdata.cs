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
                return new Employee { FirstName = "Thomas", LastName = "Galliker", Birthdate = new DateTime(1986, 07, 11), EmployementDate = new DateTime(2000, 1, 1)};
            }

            public static Employee CreateEmployee2()
            {
                return new Employee { FirstName = "Fritz", LastName = "Müller", Birthdate = new DateTime(1990, 01, 01)};
            }

            public static Employee CreateEmployee3()
            {
                return new Employee { FirstName = "Lorem", LastName = "Ipsum", Birthdate = new DateTime(2000, 12, 31) };
            }
        }

        public static class Students
        {
            public static Student CreateStudent1()
            {
                return new Student { FirstName = "Carson", LastName = "Alexander", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2005-09-01"), CountryId = "CH" };
            }

            public static Student CreateStudent2()
            {
                return new Student { FirstName = "Meredith", LastName = "Alonso", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2002-09-01"), CountryId = "ES" };
            }

            public static Student CreateStudent3()
            {
                return new Student { FirstName = "Arturo", LastName = "Anand", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2003-09-01"), CountryId = "ES" };
            }

            public static Student CreateStudent4()
            {
                return new Student { FirstName = "Gytis", LastName = "Barzdukas", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2002-09-01"), CountryId = "GR" };
            }

            public static Student CreateStudent5()
            {
                return new Student { FirstName = "Yan", LastName = "Li", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2002-09-01"), CountryId = "CN" };
            }

            public static Student CreateStudent6()
            {
                return new Student { FirstName = "Peggy", LastName = "Justice", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2001-09-01"), CountryId = "US" };
            }

            public static Student CreateStudent7()
            {
                return new Student { FirstName = "Laura", LastName = "Norman", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2003-09-01"), CountryId = "US" };
            }

            public static Student CreateStudent8()
            {
                return new Student { FirstName = "Nino", LastName = "Olivetto", Birthdate = new DateTime(1990, 1, 1), EnrollmentDate = DateTime.Parse("2005-09-01"), CountryId = "IT" };
            }
        }

        public static class Departments
        {
            public static Department CreateDepartmentHumanResources()
            {
                return new Department { Name = "Human Resources", };
            }

            public static Department CreateFacultyOfTheology()
            {
                return new Department { Name = "Faculty of Theology", };
            }

            public static Department CreateFacultyOfLaw()
            {
                return new Department { Name = "Faculty of Law", };
            }

            public static Department CreateFacultyOfMedicine()
            {
                return new Department { Name = "Faculty of Medicine", };
            }
        }

        public static class Countries
        {
            public static Country CreateCountrySwitzerland()
            {
                return new Country { Id = "CH", Name = "Switzerland" };
            }
        }
    }
}