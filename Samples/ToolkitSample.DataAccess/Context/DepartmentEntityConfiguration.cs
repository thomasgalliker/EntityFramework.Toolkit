using System.Data.Entity.ModelConfiguration;
using System.Data.Extensions.Extensions;

namespace ToolkitSample.DataAccess.Context
{
    public class DepartmentEntityConfiguration : EntityTypeConfiguration<Model.Department>
    {
        public DepartmentEntityConfiguration()
        {
            this.HasKey(d => d.Id);

            this.Property(d => d.Name).IsRequired();
            this.Property(d => d.Name).HasMaxLength(255);
            this.Property(d => d.Name).IsUnique();

            ////this.HasMany(d => d.Employees)
            ////    .WithOptional(e => e.Department);

            this.HasRequired(d => d.Leader)
                .WithMany()
                .HasForeignKey(d => d.LeaderId);

            this.HasOptional(d => d.Leader);
        }
    }
}