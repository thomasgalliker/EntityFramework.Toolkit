using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class PersonEntityConfiguration<TPerson> : EntityTypeConfiguration<TPerson> where TPerson : Person
    {
        public PersonEntityConfiguration()
        {
            this.HasKey(d => d.Id);

            this.Property(e => e.LastName).IsRequired().HasMaxLength(255);

            this.Property(e => e.FirstName).IsRequired().HasMaxLength(255);

            this.Property(e => e.Birthdate).IsRequired();

            this.Property(e => e.CreatedDate).IsRequired();
            this.Property(e => e.UpdatedDate).IsOptional();
            
            this.HasOptional(t => t.Country)
                .WithMany()
                .HasForeignKey(d => d.CountryId);

            this.Property(e => e.RowVersion)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .HasMaxLength(8)
                .IsRowVersion()
                .IsRequired();

            this.ToTable(nameof(Person));
        }
    }
}