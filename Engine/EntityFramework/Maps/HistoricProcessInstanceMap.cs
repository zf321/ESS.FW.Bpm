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
    public class HistoricProcessInstanceMap : IEntityTypeConfiguration<HistoricProcessInstanceEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricProcessInstanceEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_PROCINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.BusinessKey).HasColumnName("BUSINESS_KEY");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            //builder.Property(c => c.ProcessDefinitionName).HasColumnName("PROC_DEF_NAME");
            builder.Property(c => c.StartTime).HasColumnName("START_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Property(c => c.DurationInMillis).HasColumnName("DURATION");
            builder.Property(c => c.StartUserId).HasColumnName("START_USER_ID");
            builder.Property(c => c.StartActivityId).HasColumnName("START_ACT_ID");
            builder.Property(c => c.EndActivityId).HasColumnName("END_ACT_ID");
            builder.Property(c => c.SuperProcessInstanceId).HasColumnName("SUPER_PROCESS_INSTANCE_ID");
            builder.Property(c => c.SuperCaseInstanceId).HasColumnName("SUPER_CASE_INSTANCE_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.DeleteReason).HasColumnName("DELETE_REASON");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.State).HasColumnName("STATE");
            builder.Ignore(m => m.SequenceCounter)
                .Ignore(m=>m.EventType)
                .Ignore(m=>m.CaseExecutionId)
                .Ignore(m=>m.CaseDefinitionId)
                .Ignore(m=>m.CaseDefinitionKey)
                .Ignore(m=>m.CaseDefinitionName)
                .Ignore(m=>m.ProcessDefinitionVersion)
                .Ignore(m=>m.ProcessDefinitionName)
                .Ignore(m=>m.ExecutionId)
                ;
        }
        
    }
}
