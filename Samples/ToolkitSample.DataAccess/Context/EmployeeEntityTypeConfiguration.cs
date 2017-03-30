using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeEntityTypeConfiguration : PersonEntityConfiguration<Employee>
    {
        public EmployeeEntityTypeConfiguration()
        {
            this.Property(e => e.EmployementDate).IsOptional();

            this.HasOptional(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            this.Property(e => e.PropertyA);
            this.Property(e => e.PropertyB);

            //this.Unique(e => e.PropertyA, e => e.PropertyB);

            this.ToTable(nameof(Employee));
        }
    }
}