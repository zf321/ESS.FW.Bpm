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
    public class MeterLogMap : IEntityTypeConfiguration<MeterLogEntity>
    {
        public void Configure(EntityTypeBuilder<MeterLogEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_METER_LOG");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Reporter).HasColumnName("REPORTER");
            builder.Property(c => c.Value).HasColumnName("VALUE");
            builder.Property(c => c.Milliseconds).HasColumnName("MILLISECONDS");
        }
        
    }
}
