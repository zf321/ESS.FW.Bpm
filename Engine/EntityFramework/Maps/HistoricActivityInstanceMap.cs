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
    public class HistoricActivityInstanceMap : IEntityTypeConfiguration<HistoricActivityInstanceEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricActivityInstanceEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_ACTINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ParentActivityInstanceId).HasColumnName("PARENT_ACT_INST_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.CalledProcessInstanceId).HasColumnName("CALL_PROC_INST_ID");
            builder.Property(c => c.CalledCaseInstanceId).HasColumnName("CALL_CASE_INST_ID");
            builder.Property(c => c.ActivityName).HasColumnName("ACT_NAME");
            builder.Property(c => c.ActivityType).HasColumnName("ACT_TYPE");
            builder.Property(c => c.TaskAssignee).HasColumnName("ASSIGNEE");
            builder.Property(c => c.StartTime).HasColumnName("START_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Property(c => c.DurationInMillis).HasColumnName("DURATION");
            builder.Property(c => c.ActivityInstanceState).HasColumnName("ACT_INST_STATE");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.EventType)
                .Ignore(m=>m.CaseExecutionId)
                .Ignore(m=>m.CaseDefinitionId)
                .Ignore(m=>m.CaseDefinitionKey)
                .Ignore(m=>m.CaseDefinitionName)
                .Ignore(m=>m.ProcessDefinitionVersion)
                .Ignore(m=>m.ProcessDefinitionName)
                .Ignore(m=>m.CaseInstanceId)
                .Ignore(m=>m.ActivityInstanceId)
                ;
        }
        
    }
}
