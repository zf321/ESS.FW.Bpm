using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{

    public class DecisionDefinitionMap : IEntityTypeConfiguration<DecisionDefinitionEntity>
    {
        public void Configure(EntityTypeBuilder<DecisionDefinitionEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RE_DECISION_DEF");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("ID");
            builder.Property(m => m.Revision).HasColumnName("REV");
            builder.Property(m => m.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(m => m.ResourceName).HasColumnName("RESOURCE_NAME");
            builder.Property(m => m.DiagramResourceName).HasColumnName("DGRM_RESOURCE_NAME");
            builder.Property(m => m.DecisionRequirementsDefinitionId).HasColumnName("DEC_REQ_ID");
            builder.Property(m => m.DecisionRequirementsDefinitionKey).HasColumnName("DEC_REQ_KEY");
            builder.Property(m => m.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.PreviousDecisionDefinitionId);
            builder.Ignore(c => c.HistoryTimeToLive);
        }
        
    }
}
