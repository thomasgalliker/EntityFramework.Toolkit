using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class StudentEntityConfiguration : PersonEntityConfiguration<Student>
    {
        public StudentEntityConfiguration()
        {
            this.Property(e => e.EnrollmentDate).IsRequired();

            this.ToTable(nameof(Student));
        }
    }
}