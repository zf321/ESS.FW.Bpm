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
    public class HistoricIdentityLinkLogMap : IEntityTypeConfiguration<HistoricIdentityLinkLogEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricIdentityLinkLogEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_IDENTITYLINK");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.Time).HasColumnName("TIMESTAMP");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.GroupId).HasColumnName("GROUP_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.OperationType).HasColumnName("OPERATION_TYPE");
            builder.Property(c => c.AssignerId).HasColumnName("ASSIGNER_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.SequenceCounter);
            builder.Ignore(m => m.CaseExecutionId);
            builder.Ignore(m => m.CaseInstanceId);
            builder.Ignore(m => m.CaseDefinitionId);
            builder.Ignore(m => m.CaseDefinitionKey);
            builder.Ignore(m => m.CaseDefinitionName);
            builder.Ignore(m => m.ProcessDefinitionVersion);
            builder.Ignore(m => m.ProcessDefinitionName);
            builder.Ignore(m => m.ExecutionId);
            builder.Ignore(m => m.ProcessInstanceId);
        }
        
    }
}
