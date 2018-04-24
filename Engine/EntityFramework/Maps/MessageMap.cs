//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using System;
//using System.Collections.Generic;
//
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
//{
//    public class MessageMap:IEntityTypeConfiguration<MessageEntity>
//    {
//        public MessageMap()
//        {
//            builder.ToTable("TB_GOS_BPM_RU_JOB");
//            builder.HasKey(c => c.Id);
//            builder.Property(c => c.Id).HasColumnName("ID");
//            builder.Property(c => c.Revision).HasColumnName("REV");
//            builder.Property(c => c.LockOwner).HasColumnName("LOCK_OWNER");
//            builder.Property(c => c.LockExpirationTime).HasColumnName("LOCK_EXP_TIME");
//            builder.Property(c => c.Exclusive).HasColumnName("EXCLUSIVE_");
//            builder.Property(c => c.ExecutionId).HasColumnName("EXECUTION_ID");
//            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROCESS_INSTANCE_ID");
//            builder.Property(c => c.ProcessDefinitionId).HasColumnName("PROCESS_DEF_ID");
//            builder.Property(c => c.ProcessDefinitionKey).HasColumnName("PROCESS_DEF_KEY");
//            builder.Property(c => c.RetriesFromPersistence).HasColumnName("RETRIES");
//            builder.Property(c => c.ExceptionByteArrayId).HasColumnName("EXCEPTION_STACK_ID");
//            builder.Property(c => c.ExceptionMessage).HasColumnName("EXCEPTION_MSG");
//            builder.Property(c => c.Duedate).HasColumnName("DUEDATE");
//            builder.Property(c => c.JobHandlerType).HasColumnName("HANDLER_TYPE");
//            builder.Property(c => c.JobHandlerConfigurationRaw).HasColumnName("HANDLER_CFG");
//            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
//            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
//            builder.Property(c => c.JobDefinitionId).HasColumnName("JOB_DEF_ID");
//            builder.Property(c => c.Priority).HasColumnName("PRIORITY");
//            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
//            builder.Property(c => c.SequenceCounter).HasColumnName("SEQUENCE_COUNTER");
//            builder.Property(c => c.Type).HasColumnName("TYPE");
//            builder.Property(c => c.Repeat).HasColumnName("REPEAT");
//        }
//    }
//}
