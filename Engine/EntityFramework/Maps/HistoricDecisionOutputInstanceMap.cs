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
    public class HistoricDecisionOutputInstanceMap : IEntityTypeConfiguration<HistoricDecisionOutputInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricDecisionOutputInstanceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_DEC_OUT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.DecisionInstanceId).HasColumnName("DEC_INST_ID");
            builder.Property(c => c.ClauseId).HasColumnName("CLAUSE_ID");
            builder.Property(c => c.ClauseName).HasColumnName("CLAUSE_NAME");
            builder.Property(c => c.RuleId).HasColumnName("RULE_ID");
            builder.Property(c => c.RuleOrder).HasColumnName("RULE_ORDER");
            builder.Property(c => c.VariableName).HasColumnName("VAR_NAME");
            builder.Property(c => c.SerializerName).HasColumnName("VAR_TYPE");
            builder.Property(c => c.ByteArrayId).HasColumnName("BYTEARRAY_ID");
            builder.Property(c => c.DoubleValue).HasColumnName("DOUBLE");
            builder.Property(c => c.TextValue).HasColumnName("TEXT");
            builder.Property(c => c.TextValue2).HasColumnName("TEXT2");
            builder.Property(c => c.LongValue).HasColumnName("LONG_");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
