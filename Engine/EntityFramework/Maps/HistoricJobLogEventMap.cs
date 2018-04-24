using ESS.FW.Bpm.Engine.History.Impl.Event;
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
    public class HistoricJobLogEventMap : IEntityTypeConfiguration<HistoricJobLogEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricJobLogEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_JOB_LOG");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.TimeStamp).HasColumnName("TIMESTAMP");
            builder.Property(c => c.JobId).HasColumnName("JOB_ID");
            builder.Property(c => c.JobDueDate).HasColumnName("JOB_DUEDATE");
            builder.Property(c => c.JobRetries).HasColumnName("JOB_RETRIES");
            builder.Property(c => c.JobPriority).HasColumnName("JOB_PRIORITY");
            builder.Property(c => c.JobExceptionMessage).HasColumnName("JOB_EXCEPTION_MSG");
            builder.Property(c => c.ExceptionByteArrayId).HasColumnName("JOB_EXCEPTION_STACK_ID");
            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
            builder.Property(c => c.JobDefinitionType).HasColumnName("JOB_DEF_TYPE");
            builder.Property(c => c.JobDefinitionConfiguration).HasColumnName("JOB_DEF_CONFIGURATION");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROCESS_INSTANCE_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROCESS_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROCESS_DEF_KEY");
            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(c => c.State).HasColumnName("JOB_STATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            builder.Ignore(m => m.EventType)
            .Ignore(m => m.CaseExecutionId)
            .Ignore(m => m.CaseInstanceId)
            .Ignore(m => m.CaseDefinitionKey)
            .Ignore(m => m.CaseDefinitionName)
            .Ignore(m => m.CaseDefinitionId)
            .Ignore(m => m.ProcessDefinitionVersion)
            .Ignore(m => m.ProcessDefinitionName);
            ;
        }
        
    }
}
