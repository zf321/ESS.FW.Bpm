using ESS.FW.Bpm.Engine.Batch.Impl.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class HistoricBatchMap : IEntityTypeConfiguration<HistoricBatchEntity>
    {
        public void Configure(EntityTypeBuilder<HistoricBatchEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_BATCH");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.TotalJobs).HasColumnName("TOTAL_JOBS");
            builder.Property(c => c.BatchJobsPerSeed).HasColumnName("JOBS_PER_SEED");
            builder.Property(c => c.InvocationsPerBatchJob).HasColumnName("INVOCATIONS_PER_JOB");
            builder.Property(c => c.SeedJobDefinitionId).HasColumnName("SEED_JOB_DEF_ID");
            builder.Property(c => c.MonitorJobDefinitionId).HasColumnName("MONITOR_JOB_DEF_ID");
            builder.Property(c => c.BatchJobDefinitionId).HasColumnName("BATCH_JOB_DEF_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.StartTime).HasColumnName("START_TIME");
            builder.Property(c => c.EndTime).HasColumnName("END_TIME");
            builder.Ignore(m => m.ProcessInstanceId)
                .Ignore(m=>m.ExecutionId)
                .Ignore(m=>m.ProcessDefinitionId)
                .Ignore(m=>m.ProcessDefinitionKey)
                .Ignore(m=>m.SequenceCounter)
                ;
        }
        
    }
}
