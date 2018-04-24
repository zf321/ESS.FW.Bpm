using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricIncidentMap : IEntityTypeConfiguration<HistoricIncidentEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricIncidentEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_INCIDENT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.CreateTime).HasColumnName("CREATE_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Property(c => c.IncidentMessage).HasColumnName("INCIDENT_MSG");
            builder.Property(c => c.IncidentType).HasColumnName("INCIDENT_TYPE");
            builder.Property(c => c.ActivityId).HasColumnName("ACTIVITY_ID");
            builder.Property(c => c.CauseIncidentId).HasColumnName("CAUSE_INCIDENT_ID");
            builder.Property(c => c.RootCauseIncidentId).HasColumnName("ROOT_CAUSE_INCIDENT_ID");
            builder.Property(c => c.Configuration).HasColumnName("CONFIGURATION");
            builder.Property(c => c.IncidentState).HasColumnName("INCIDENT_STATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
            builder.Ignore(m => m.SequenceCounter)
                .Ignore(m => m.EventType)
                .Ignore(m=>m.CaseInstanceId)
                .Ignore(m => m.CaseExecutionId)
                .Ignore(m => m.CaseDefinitionId)
                .Ignore(m => m.CaseDefinitionKey)
                .Ignore(m => m.CaseDefinitionName)
                .Ignore(m => m.ProcessDefinitionVersion)
                .Ignore(m => m.ProcessDefinitionName);
        }
        
    }
}
