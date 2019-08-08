﻿using EntityFramework.Toolkit.Tests.Stubs;
using FluentAssertions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Testing;
using ToolkitSample.DataAccess;
using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.DataAccess.Migrations;
using ToolkitSample.DataAccess.Repository;
using ToolkitSample.DataAccess.Seed;
using ToolkitSample.Model;
using Xunit;
using Xunit.Abstractions;


namespace EntityFramework.Toolkit.Tests.Repository
{
    public class PersonRepositoryTests : ContextTestBase<EmployeeContext>
    {
        public PersonRepositoryTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(new List<IDataSeed> { new CountryDataSeed() })),
                  log: testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldIncludeAllNavigationPropertiesOfBaseEntity()
        {
            // Arrange
            var department = Testdata.Departments.CreateFacultyOfLaw();

            var employee1 = Testdata.Employees.CreateEmployee1();
            employee1.Department = department;

            var employee2 = Testdata.Employees.CreateEmployee2();
            employee2.Department = department;

            var student1 = Testdata.Students.CreateStudent1();
            var student2 = Testdata.Students.CreateStudent2();

            using (IPersonRepository personRepository = new PersonRepository(this.CreateContext()))
            {
                personRepository.Add(employee1);
                personRepository.Add(employee2);
                personRepository.Add(student1);
                personRepository.Add(student2);
                personRepository.Save();
            }

            // Act
            List<Person> returnedPersons;
            using (IPersonRepository personRepository = new PersonRepository(this.CreateContext()))
            {
                returnedPersons = personRepository.Query().IncludeAllRelated().ToList();
            }

            // Assert
            returnedPersons.Should().NotBeNull();
            returnedPersons.Should().HaveCount(4);
        }

        [Fact]
        public void ShouldIncludeAllNavigationPropertiesOfDerivedEntities()
        {
            // Arrange
            var department = Testdata.Departments.CreateFacultyOfLaw();

            var employee1 = Testdata.Employees.CreateEmployee1();
            employee1.Department = department;
            employee1.CountryId = "CH";

            var employee2 = Testdata.Employees.CreateEmployee2();
            employee2.Department = department;
            employee2.CountryId = "CH";

            var student1 = Testdata.Students.CreateStudent1();
            student1.CountryId = "CH";

            var student2 = Testdata.Students.CreateStudent2();
            student2.CountryId = "CH";

            using (IPersonRepository personRepository = new PersonRepository(this.CreateContext()))
            {
                personRepository.Add(employee1);
                personRepository.Add(employee2);
                personRepository.Add(student1);
                personRepository.Add(student2);
                personRepository.Save();
            }

            // Act
            IEnumerable<Person> returnedPersons;
            using (IPersonRepository personRepository = new PersonRepository(this.CreateContext()))
            {
                returnedPersons = personRepository.Query().IncludeAllRelatedDerived().ToList();
            }

            // Assert
            using (IPersonRepository personRepository = new PersonRepository(this.CreateContext()))
            {
                var expectedPersons = personRepository.Get().OfType<Employee>()
                    .Include(e => e.Country)
                    .Include(e => e.Department).As<IEnumerable<Person>>().ToList()
                  .Union(personRepository.Get().OfType<Student>()
                  .Include(s => s.Country).As<IEnumerable<Person>>().ToList()).ToList();

                returnedPersons.ShouldAllBeEquivalentTo(expectedPersons);
            }

            returnedPersons.Should().NotBeNull();
            returnedPersons.Should().HaveCount(4);

            returnedPersons.OfType<Employee>().Should().HaveCount(2);
            returnedPersons.OfType<Employee>().All(e => e.Department != null).Should().BeTrue();
            returnedPersons.OfType<Employee>().All(e => e.Country != null).Should().BeTrue();

            returnedPersons.OfType<Student>().Should().HaveCount(2);
            returnedPersons.OfType<Student>().All(e => e.Country != null).Should().BeTrue();
        }
    }
}