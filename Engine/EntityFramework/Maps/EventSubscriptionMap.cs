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
    public class EventSubscriptionMap : IEntityTypeConfiguration<EventSubscriptionEntity>
    {
        public void Configure(EntityTypeBuilder<EventSubscriptionEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_EVENT_SUBSCR");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.EventType).HasColumnName("EVENT_TYPE");
            builder.Property(c => c.EventName).HasColumnName("EVENT_NAME");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACTIVITY_ID");
            builder.Property(c => c.Configuration).HasColumnName("CONFIGURATION");
            builder.Property(c => c.Created).HasColumnName("CREATED");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
