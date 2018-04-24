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
    public class HistoricExternalTaskLogMap : IEntityTypeConfiguration<HistoricExternalTaskLogEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricExternalTaskLogEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_EXT_TASK_LOG");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.TimeStamp).HasColumnName("TIMESTAMP");
            builder.Property(m => m.ExternalTaskId).HasColumnName("EXT_TASK_ID");
            //builder.Property(c => c.TaskId).HasColumnName("TASK_ID_");
            builder.Property(c => c.Retries).HasColumnName("RETRIES");
            builder.Property(c => c.TopicName).HasColumnName("TOPIC_NAME");
            builder.Property(c => c.WorkerId).HasColumnName("WORKER_ID");
            builder.Property(c => c.Priority).HasColumnName("PRIORITY");
            builder.Property(c => c.ErrorMessage).HasColumnName("ERROR_MSG");
            builder.Property(c => c.ErrorDetailsByteArrayId).HasColumnName("ERROR_DETAILS_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INSTANCE_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.State).HasColumnName("STATE");
            builder.Property(m => m.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Ignore(m => m.SequenceCounter);
            builder.Ignore(m => m.CaseExecutionId);
            builder.Ignore(m => m.CaseInstanceId);
            builder.Ignore(m => m.CaseDefinitionId);
            builder.Ignore(m => m.CaseDefinitionKey);
            builder.Ignore(m => m.CaseDefinitionName);
            builder.Ignore(m => m.ProcessDefinitionVersion);
            builder.Ignore(m => m.ProcessDefinitionName);
            builder.Ignore(m => m.ErrorDetails);
        }
        
    }
}
