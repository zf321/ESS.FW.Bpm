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
    public class HistoricVariableInstanceMap : IEntityTypeConfiguration<HistoricVariableInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricVariableInstanceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_VARINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.CaseDefinitionKey).HasColumnName("CASE_DEF_KEY");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.SerializerName).HasColumnName("VAR_TYPE");
            builder.Property(c => c.ByteArrayId).HasColumnName("BYTEARRAY_ID");
            builder.Property(c => c.DoubleValue).HasColumnName("DOUBLE");
            builder.Property(c => c.TextValue).HasColumnName("TEXT");
            builder.Property(c => c.TextValue2).HasColumnName("TEXT2");
            builder.Property(c => c.LongValue).HasColumnName("LONG_");
            builder.Ignore(c => c.ByteArrayValue);
            builder.Ignore(c => c.StartTime);
            builder.Ignore(c => c.EndTime);
        }
        
    }
}
