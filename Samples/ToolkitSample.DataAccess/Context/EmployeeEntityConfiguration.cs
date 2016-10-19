using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeEntityConfiguration : PersonEntityConfiguration<Employee>
    {
        public EmployeeEntityConfiguration()
        {
            this.Property(e => e.EmployementDate).IsOptional();

            this.HasOptional(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            this.ToTable(nameof(Employee));
        }
    }
}