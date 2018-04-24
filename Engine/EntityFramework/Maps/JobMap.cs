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
    public class JobMap : IEntityTypeConfiguration<JobEntity>
    {
        public void Configure(EntityTypeBuilder<JobEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_JOB");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.LockOwner).HasColumnName("LOCK_OWNER");
            builder.Property(c => c.LockExpirationTime).HasColumnName("LOCK_EXP_TIME");
            builder.Property(c => c.Exclusive).HasColumnName("EXCLUSIVE_");
            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROCESS_INSTANCE_ID");
            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROCESS_DEF_ID");
            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROCESS_DEF_KEY");
            builder.Property(c => c.RetriesFromPersistence).HasColumnName("RETRIES");
            builder.Property(c => c.ExceptionByteArrayId).HasColumnName("EXCEPTION_STACK_ID");
            builder.Property(c => c.ExceptionMessage).HasColumnName("EXCEPTION_MSG");
            builder.Property(c => c.Duedate).HasColumnName("DUEDATE");
            builder.Property(c => c.JobHandlerType).HasColumnName("HANDLER_TYPE");
            builder.Property(c => c.JobHandlerConfigurationRaw).HasColumnName("HANDLER_CFG");
            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
            builder.Property(c => c.Priority).HasColumnName("PRIORITY");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
            //builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.Repeat).HasColumnName("REPEAT");

            //Map<TimerEntity>(c =>{c.Requires("TYPE").HasValue("timer");});
            //Map<MessageEntity>(c => { c.Requires("TYPE").HasValue("message"); });
            //Map<EverLivingJobEntity>(c => { c.Requires("TYPE").HasValue("ever-living"); });

            builder.Property(c => c.Revision).IsConcurrencyToken();
        }
        
    }
    //public class TimerMap : IEntityTypeConfiguration<TimerEntity>
    //{
    //    public TimerMap()
    //    {
    //        builder.ToTable("ACT_RU_JOB");
    //        builder.Property(c => c.Type).HasColumnName("TYPE_");
    //        builder.Property(c => c.Repeat).HasColumnName("REPEAT_");
    //    }
    //}
}
