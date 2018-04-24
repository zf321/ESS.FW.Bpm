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
    public class ExecutionMap : IEntityTypeConfiguration<ExecutionEntity>
    {
        public void Configure(EntityTypeBuilder<ExecutionEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_EXECUTION");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.BusinessKey).HasColumnName("BUSINESS_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.IsActive).HasColumnName("IS_ACTIVE");
            builder.Property(c => c.IsConcurrent).HasColumnName("IS_CONCURRENT");
            builder.Property(c => c.IsScope).HasColumnName("IS_SCOPE");
            builder.Property(c => c.IsEventScope).HasColumnName("IS_EVENT_SCOPE");
            builder.Property(c => c.ParentId).HasColumnName("PARENT_ID");
            builder.Property(c => c.SuperExecutionId).HasColumnName("SUPER_EXEC");
            builder.Property(c => c.SuperCaseExecutionId).HasColumnName("SUPER_CASE_EXEC");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.CachedEntityState).HasColumnName("CACHED_ENT_STATE");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");

            //builder.HasOptional(c => c.ParentExecutionEf).WithMany().HasForeignKey(c => c.ParentIdEf);
            //builder.HasOptional(m => m.ProcessDefinitionEf).WithMany().HasForeignKey(m => m.ProcessDefinitionIdEf);
            //builder.HasOptional(m => m.ProcessInstanceEf).WithMany().HasForeignKey(m => m.ProcessInstanceIdEf);
            //builder.HasOptional(m => m.SuperExecutionEf).WithMany().HasForeignKey(m => m.SuperExecutionId);


            //builder.HasOptional(c => c.ProcessInstance).WithMany().HasForeignKey(c => c.ProcessInstanceIdEf);
            //builder.HasOptional(c => c.SubProcessInstance).WithMany().HasForeignKey(c => c.SuperExecutionId);

            //builder.Ignore(m => m.VariableInstancesLocal);  //基类包含Ilist<VariableInstanceEntity>
            //Ignore(c => c.ProcessDefinition);
            //Ignore(c => c.ProcessDefinitionEf);
        }
        
    }
}
