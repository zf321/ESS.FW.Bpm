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
    public class JobDefinitionMap : IEntityTypeConfiguration<JobDefinitionEntity>
    {
        public void Configure(EntityTypeBuilder<JobDefinitionEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_JOBDEF");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.JobType).HasColumnName("JOB_TYPE");
            builder.Property(c => c.JobConfiguration).HasColumnName("JOB_CONFIGURATION");
            builder.Property(c => c.JobPriority).HasColumnName("JOB_PRIORITY");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
