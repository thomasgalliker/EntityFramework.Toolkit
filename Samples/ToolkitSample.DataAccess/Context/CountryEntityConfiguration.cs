using System.Data.Entity.ModelConfiguration;
using EntityFramework.Toolkit.EF6.Extensions;

namespace ToolkitSample.DataAccess.Context
{
    public class CountryEntityConfiguration : EntityTypeConfiguration<Model.Country>
    {
        public CountryEntityConfiguration()
        {
            this.HasKey(d => d.Id);

            this.Property(t => t.Id)
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnique();
        }
    }
}