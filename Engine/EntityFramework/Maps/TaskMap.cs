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
    public class TaskMap : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_TASK");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.NameWithoutCascade).HasColumnName("NAME");
            builder.Property(c => c.ParentTaskIdWithoutCascade).HasColumnName("PARENT_TASK_ID");
            builder.Property(c => c.DescriptionWithoutCascade).HasColumnName("DESCRIPTION");
            builder.Property(c => c.PriorityWithoutCascade).HasColumnName("PRIORITY");
            builder.Property(c => c.CreateTime).HasColumnName("CREATE_TIME");
            builder.Property(c => c.OwnerWithoutCascade).HasColumnName("OWNER");
            builder.Property(c => c.AssigneeWithoutCascade).HasColumnName("ASSIGNEE");
            builder.Property(c => c.DelegationStateString).HasColumnName("DELEGATION");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.CaseExecutionId).HasColumnName("CASE_EXECUTION_ID");
            builder.Property(c => c.CaseInstanceIdWithoutCascade).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.TaskDefinitionKey).HasColumnName("TASK_DEF_KEY");
            builder.Property(c => c.DueDateWithoutCascade).HasColumnName("DUE_DATE");
            builder.Property(c => c.FollowUpDateWithoutCascade).HasColumnName("FOLLOW_UP_DATE");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.Name);

            builder.Property(c => c.Revision)
                .IsConcurrencyToken();

            builder.HasMany(c => c.IdentityLinks);//.WithOptional().HasForeignKey(c => c.TaskId);
        }
        
    }
}
