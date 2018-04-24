using ESS.FW.Bpm.Engine.History.Impl.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricDecisionInstanceMap : IEntityTypeConfiguration<HistoricDecisionInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricDecisionInstanceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_DECINST");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.DecisionDefinitionId).HasColumnName("DEC_DEF_ID");
            builder.Property(c => c.DecisionDefinitionKey).HasColumnName("DEC_DEF_KEY");
            builder.Property(c => c.DecisionDefinitionName).HasColumnName("DEC_DEF_NAME");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROC_DEF_KEY");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.CaseDefinitionKey).HasColumnName("CASE_DEF_KEY");
            builder.Property(c => c.CaseDefinitionId).HasColumnName("CASE_DEF_ID");
            builder.Property(c => c.CaseInstanceId).HasColumnName("CASE_INST_ID");
            builder.Property(c => c.ActivityInstanceId).HasColumnName("ACT_INST_ID");
            builder.Property(c => c.ActivityId).HasColumnName("ACT_ID");
            builder.Property(c => c.EvaluationTime).HasColumnName("EVAL_TIME");
            builder.Property(c => c.CollectResultValue).HasColumnName("COLLECT_VALUE");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.RootDecisionInstanceId).HasColumnName("ROOT_DEC_INST_ID");
            builder.Property(c => c.DecisionRequirementsDefinitionId).HasColumnName("DEC_REQ_ID");
            builder.Property(c => c.DecisionRequirementsDefinitionKey).HasColumnName("DEC_REQ_KEY");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Ignore(m => m.ProcessDefinitionName);
            builder.Ignore(m => m.ProcessDefinitionVersion);
            //Ignore(m => m.CaseDefinitionKey);
            builder.Ignore(m => m.CaseDefinitionName);
            builder.Ignore(m => m.CaseExecutionId);
            builder.Ignore(m => m.SequenceCounter);
            builder.Ignore(m => m.ExecutionId);
        }
        
    }
}
