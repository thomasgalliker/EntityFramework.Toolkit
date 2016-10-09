using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeEntityConfiguration : EntityTypeConfiguration<Model.Employee>
    {
        public EmployeeEntityConfiguration()
        {
            this.HasKey(d => d.Id);

            this.Property(e => e.LastName).IsRequired();
            this.Property(e => e.LastName).HasMaxLength(255);

            this.Property(e => e.FirstName).IsRequired();
            this.Property(e => e.FirstName).HasMaxLength(255);

            this.Property(e => e.Birthdate).IsRequired();

            this.HasOptional(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            this.HasOptional(t => t.Country)
                .WithMany()
                .HasForeignKey(d => d.CountryId);

            this.Property(e => e.RowVersion)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .HasMaxLength(8)
                .IsRowVersion()
                .IsRequired();
        }
    }
}