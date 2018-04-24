using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricCaseInstanceMap : IEntityTypeConfiguration<HistoricCaseInstanceEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricCaseInstanceEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_CASEINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.BusinessKey).HasColumnName("BUSINESS_KEY");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseDefinitionKey).HasColumnName("KEY");
            builder.Property(c => c.CaseDefinitionName).HasColumnName("NAME");
            builder.Property(c => c.StartTime).HasColumnName("CREATE_TIME");
            builder.Property(c => c.EndTime).HasColumnName("CLOSE_TIME");
            builder.Property(c => c.DurationInMillis).HasColumnName("DURATION");
            builder.Property(c => c.State).HasColumnName("STATE");
            builder.Property(c => c.CreateUserId).HasColumnName("CREATE_USER_ID");
            builder.Property(c => c.SuperCaseInstanceId).HasColumnName("SUPER_CASE_INSTANCE_ID");
            builder.Property(c => c.SuperProcessInstanceId).HasColumnName("SUPER_PROCESS_INSTANCE_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
