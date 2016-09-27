using FluentAssertions;
using ToolkitSample.Model;

namespace EntityFramework.Toolkit.Tests.Extensions
{
    internal static class AssertionExtensions
    {
        internal static void ShouldBeEquivalentTo(this Employee subject, Employee expectedEmployee)
        {
            subject.ShouldBeEquivalentTo(expectedEmployee, options => options.IncludingAllDeclaredProperties()
                                                                             .IgnoringCyclicReferences()
                                                                             .Excluding(e => e.Id)
                                                                             .Excluding(e => e.Department)
                                                                             .Excluding(e => e.DepartmentId)
                                                                             .Excluding(e => e.RowVersion));
        }
    }
}
