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
    public class HistoricTaskInstanceMap : IEntityTypeConfiguration<HistoricTaskInstanceEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricTaskInstanceEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_TASKINST");
            builder.HasKey(c => c.Id);
            builder.Property(c=>c.Id).HasColumnName("ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.CaseDefinitionKey).HasColumnName("CASE_DEF_KEY");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.ParentTaskId).HasColumnName("PARENT_TASK_ID");
            builder.Property(c => c.Description).HasColumnName("DESCRIPTION");
            builder.Property(c => c.Owner).HasColumnName("OWNER");
            builder.Property(c => c.Assignee).HasColumnName("ASSIGNEE");
            builder.Property(c => c.StartTime).HasColumnName("START_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Property(c => c.DurationInMillis).HasColumnName("DURATION");
            builder.Property(c => c.DeleteReason).HasColumnName("DELETE_REASON");
            builder.Property(c => c.TaskDefinitionKey).HasColumnName("TASK_DEF_KEY");
            builder.Property(c => c.Priority).HasColumnName("PRIORITY");
            builder.Property(c => c.DueDate).HasColumnName("DUE_DATE");
            builder.Property(c => c.FollowUpDate).HasColumnName("FOLLOW_UP_DATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.SequenceCounter)
                .Ignore(m => m.EventType)
                .Ignore(m => m.CaseDefinitionName)
                .Ignore(m=>m.ProcessDefinitionVersion)
                .Ignore(m=>m.ProcessDefinitionName)
                .Ignore(m=>m.TaskId)
            ;
        }
        
    }
}
