
using System.Data.Entity.ModelConfiguration;
using EntityFramework.Toolkit.EF6.Extensions;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class RoomConfiguration : EntityTypeConfiguration<Room>
    {
        public RoomConfiguration()
        {
            this.HasKey(d => d.Id);

            this.Property(e => e.Description).IsOptional().HasMaxLength(255);

            this.Property(e => e.Level);
            this.Property(e => e.Sector).HasMaxLength(900);

            this.Unique(e => e.Level, e => e.Sector);

            this.ToTable(nameof(Room));
        }
    }
}