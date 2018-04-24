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
    public class VariableInstanceMap:IEntityTypeConfiguration<VariableInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<VariableInstanceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_VARIABLE");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.SerializerName).HasColumnName("TYPE");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            //builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            //builder.Property(c => c.ActivityId).HasColumnName("ACTIVITY_ID");
            //builder.Property(c => c.IsActive).HasColumnName("IS_ACTIVE");
            //builder.Property(c => c.IsConcurrencyScope).HasColumnName("IS_CONCURRENCY_SCOPE");
            builder.Property(c => c.ByteArrayId).HasColumnName("BYTEARRAY_ID");
            builder.Property(c => c.DoubleValue).HasColumnName("DOUBLE");
            builder.Property(c => c.TextValue).HasColumnName("TEXT");
            builder.Property(c => c.TextValue2).HasColumnName("TEXT2");
            builder.Property(c => c.LongValue).HasColumnName("LONG_");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            builder.Property(c => c.IsConcurrentLocal).HasColumnName("IS_CONCURRENT_LOCAL");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(m => m.VariableScopeId).HasColumnName("VAR_SCOPE");

            builder.Ignore(m => m.IsConcurrencyScope)
                .Ignore(m=>m.IsActive)
                .Ignore(m=>m.ActivityId)
                .Ignore(m=>m.ActivityInstanceId)
                .Ignore(c=>c.ByteArrayValue)
                ;
            //builder.Ignore(m => m.Execution);
            //HasOptional(p => p.Execution).WithMany().HasForeignKey(p => p.ExecutionId).WillCascadeOnDelete(false);
        }
    }
}
