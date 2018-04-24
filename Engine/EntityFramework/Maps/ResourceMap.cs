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
    public class ResourceMap : IEntityTypeConfiguration<ResourceEntity>
    {
        public void Configure(EntityTypeBuilder<ResourceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_GE_BYTEARRAY");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Name).HasColumnName("NAME").IsRequired();
            builder.Property(c => c.Bytes).HasColumnName("BYTES");
            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(c => c.Generated).HasColumnName("GENERATED");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
