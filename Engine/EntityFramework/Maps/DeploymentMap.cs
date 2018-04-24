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
    public class DeploymentMap : IEntityTypeConfiguration<DeploymentEntity>
    {
        public void Configure(EntityTypeBuilder<DeploymentEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RE_DEPLOYMENT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Source).HasColumnName("SOURCE");
            builder.Property(c => c.DeploymentTime).HasColumnName("DEPLOY_TIME");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(c => c.IsNew)
                .Ignore(c => c.IsValidatingSchema)
                .Ignore(c=>c.DeployedArtifacts)
                .Ignore(c=>c.Resources)
                ;
        }
        
    }
}
