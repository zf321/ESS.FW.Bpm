using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricDecisionInputInstanceMap : IEntityTypeConfiguration<HistoricDecisionInputInstanceEntity>
    {
        public  void Configure(EntityTypeBuilder<HistoricDecisionInputInstanceEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_DEC_IN");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.DecisionInstanceId).HasColumnName("DEC_INST_ID");
            builder.Property(c => c.ClauseId).HasColumnName("CLAUSE_ID");
            builder.Property(c => c.ClauseName).HasColumnName("CLAUSE_NAME");
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
