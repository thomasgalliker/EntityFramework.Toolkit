using FluentAssertions;
using ToolkitSample.Model;

namespace EntityFramework.Toolkit.Tests.Extensions
{
    internal static class AssertionExtensions
    {
        internal static void ShouldBeEquivalentTo(this Employee subject, Employee expected)
        {
            subject.ShouldBeEquivalentTo(expected, options => options.IncludingAllRuntimeProperties()
                                                                             .ExcludingNestedObjects()
                                                                             .IgnoringCyclicReferences()
                                                                             .Excluding(e => e.Id)
                                                                             .Excluding(e => e.Department)
                                                                             .Excluding(e => e.DepartmentId)
                                                                             .Excluding(e => e.CountryId)
                                                                             .Excluding(e => e.RowVersion));
        }

        internal static void ShouldBeEquivalentTo(this Department subject, Department expected)
        {
            subject.ShouldBeEquivalentTo(expected, options => options.IncludingAllRuntimeProperties()
                                                                             .ExcludingNestedObjects()
                                                                             .IgnoringCyclicReferences()
                                                                             .Excluding(e => e.Id)
                                                                             .Excluding(e => e.Employees)
                                                                             .Excluding(e => e.Leader)
                                                                             .Excluding(e => e.LeaderId)
                                                                             .Excluding(e => e.RowVersion));
        }
    }
}
