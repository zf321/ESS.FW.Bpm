using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class FilterMap : IEntityTypeConfiguration<FilterEntity>
    {
        public void Configure(EntityTypeBuilder<FilterEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_FILTER");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.ResourceType).HasColumnName("RESOURCE_TYPE");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Owner).HasColumnName("OWNER");
            builder.Property(c => c.QueryInternal).HasColumnName("QUERY");
            builder.Property(c => c.PropertiesInternal).HasColumnName("PROPERTIES");
        }
        
    }
}
