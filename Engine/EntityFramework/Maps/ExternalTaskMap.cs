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
    public class ExternalTaskMap : IEntityTypeConfiguration<ExternalTaskEntity>
    {
        public void Configure(EntityTypeBuilder<ExternalTaskEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_EXT_TASK");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.TopicName).HasColumnName("TOPIC_NAME");
            builder.Property(c => c.WorkerId).HasColumnName("WORKER_ID");
            builder.Property(c => c.Retries).HasColumnName("RETRIES");
            builder.Property(c => c.ErrorMessage).HasColumnName("ERROR_MSG");
            builder.Property(c => c.ErrorDetailsByteArrayId).HasColumnName("ERROR_DETAILS_ID");
            builder.Property(c => c.LockExpirationTime).HasColumnName("LOCK_EXP_TIME");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.Priority).HasColumnName("PRIORITY");
        }
        
    }
}
