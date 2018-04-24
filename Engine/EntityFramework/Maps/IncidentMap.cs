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
    public class IncidentMap: IEntityTypeConfiguration<IncidentEntity>
    {
        public void Configure(EntityTypeBuilder<IncidentEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_INCIDENT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.IncidentTimestamp).HasColumnName("INCIDENT_TIMESTAMP");
            builder.Property(c => c.IncidentType).HasColumnName("INCIDENT_TYPE");
            builder.Property(c => c.IncidentMessage).HasColumnName("INCIDENT_MSG");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACTIVITY_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.CauseIncidentId).HasColumnName("CAUSE_INCIDENT_ID");
            builder.Property(c => c.RootCauseIncidentId).HasColumnName("ROOT_CAUSE_INCIDENT_ID");
            builder.Property(c => c.Configuration).HasColumnName("CONFIGURATION");
            builder.Property(m => m.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
        }
        
    }
}
