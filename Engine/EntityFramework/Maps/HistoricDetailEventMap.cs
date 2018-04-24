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
    public class HistoricDetailEventMap : IEntityTypeConfiguration<HistoricDetailEventEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricDetailEventEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_DETAIL");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.CaseDefinitionKey).HasColumnName("CASE_DEF_KEY");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.TimeStamp).HasColumnName("TIME");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            //builder.Property(c => c.Type).HasColumnName("TYPE_");
            //builder.Property(c => c.VariableName).HasColumnName("NAME_");
            //builder.Property(c => c.VariableInstanceId).HasColumnName("VAR_INST_ID_");
            //builder.Property(c => c.Revision).HasColumnName("REV_");
            //builder.Property(c => c.SerializerName).HasColumnName("VAR_TYPE_");
            //builder.Property(c => c.ByteArrayId).HasColumnName("BYTEARRAY_ID_");
            //builder.Property(c => c.DoubleValue).HasColumnName("DOUBLE_");
            //builder.Property(c => c.TextValue).HasColumnName("TEXT_");
            //builder.Property(c => c.TextValue2).HasColumnName("TEXT2_");
            //builder.Property(c => c.LongValue).HasColumnName("LONG_");
            builder.Ignore(m => m.CaseDefinitionName);
            builder.Ignore(m => m.ProcessDefinitionVersion);
            builder.Ignore(m => m.ProcessDefinitionName);
            //Map<HistoricFormPropertyEventEntity>(c =>
            //{
            //    c.Requires("TYPE").HasValue("FormProperty");
            //});
            //Map<HistoricVariableUpdateEventEntity>(c =>
            //{
            //    //c.Property(m => m.TextValue).HasColumnName("TEXT");
            //    //c.Property(m => m.TextValue2).HasColumnName("TEXT2");
            //    c.Requires("TYPE").HasValue("VariableUpdate");
            //    c.Requires(m => m.TextValue);
            //    c.Requires(m => m.TextValue2);
            //});
        }
    }
}
