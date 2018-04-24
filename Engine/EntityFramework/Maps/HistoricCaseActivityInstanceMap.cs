using ESS.FW.Bpm.Engine.History.Impl.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricCaseActivityInstanceMap : IEntityTypeConfiguration<HistoricCaseActivityInstanceEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricCaseActivityInstanceEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_CASEACTINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ParentCaseActivityInstanceId).HasColumnName("PARENT_ACT_INST_ID");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseActivityId).HasColumnName("CASE_ACT_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.CalledProcessInstanceId).HasColumnName("CALL_PROC_INST_ID");
            builder.Property(c => c.CalledCaseInstanceId).HasColumnName("CALL_CASE_INST_ID");
            builder.Property(c => c.CaseActivityName).HasColumnName("CASE_ACT_NAME");
            builder.Property(c => c.CaseActivityType).HasColumnName("CASE_ACT_TYPE");
            builder.Property(c => c.StartTime).HasColumnName("CREATE_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Property(c => c.DurationInMillis).HasColumnName("DURATION");
            builder.Property(c => c.CaseActivityInstanceState).HasColumnName("STATE");
            builder.Property(c => c.Required).HasColumnName("REQUIRED");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
