using ESS.FW.Bpm.Engine.History.Impl.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class UserOperationLogEntryEventMap : IEntityTypeConfiguration<UserOperationLogEntryEventEntity>
    {
        public void Configure(EntityTypeBuilder<UserOperationLogEntryEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_OP_LOG");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.JobId).HasColumnName("JOB_ID");
            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
            builder.Property(c => c.BatchId).HasColumnName("BATCH_ID");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.Timestamp).HasColumnName("TIMESTAMP");
            builder.Property(c => c.OperationId).HasColumnName("OPERATION_ID");
            builder.Property(c => c.OperationType).HasColumnName("OPERATION_TYPE");
            builder.Property(c => c.EntityType).HasColumnName("ENTITY_TYPE");
            builder.Property(c => c.Property).HasColumnName("PROPERTY");
            builder.Property(c => c.OrgValue).HasColumnName("ORG_VALUE");
            builder.Property(c => c.NewValue).HasColumnName("NEW_VALUE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
