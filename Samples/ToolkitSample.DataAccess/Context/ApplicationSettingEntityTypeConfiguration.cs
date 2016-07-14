using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ToolkitSample.DataAccess.Context
{
    public class ApplicationSettingEntityTypeConfiguration : EntityTypeConfiguration<Model.ApplicationSetting>
    {
        public ApplicationSettingEntityTypeConfiguration()
        {
            this.HasKey(d => d.Id);
            this.Property(d => d.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(d => d.Path).HasMaxLength(255);
        }
    }
}