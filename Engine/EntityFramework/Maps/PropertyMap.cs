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
    public class PropertyMap : IEntityTypeConfiguration<PropertyEntity>
    {
        public void Configure(EntityTypeBuilder<PropertyEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_GE_PROPERTY")
                .HasKey(c => c.Name);
            //builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Value).HasColumnName("VALUE");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Ignore(c => c.Id);
        }
        
    }
}
