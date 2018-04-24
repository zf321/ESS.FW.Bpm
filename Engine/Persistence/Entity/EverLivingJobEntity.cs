using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public class EverLivingJobEntity : JobEntity
    {
        private const long serialVersionUID = 1L;

        public const string TYPE = "ever-living";

        public override string Type => TYPE;

        protected internal override void PostExecute(CommandContext commandContext)
        {
            Init(commandContext);
            commandContext.HistoricJobLogManager.FireJobSuccessfulEvent(this);
        }

        public override void Init(CommandContext commandContext)
        {
            // clean additional data related to this job
            IJobHandler jobHandler = JobHandler;

            jobHandler?.OnDelete(JobHandlerConfiguration, this);

            //cancel the retries -> will resolve job incident if present
            Retries = commandContext.ProcessEngineConfiguration.DefaultNumberOfRetries;

            //delete the job's exception byte array and exception message
            string exceptionByteArrayIdToDelete = null;
            if (ExceptionByteArrayId != null)
            {
                exceptionByteArrayIdToDelete = ExceptionByteArrayId;
                this.ExceptionByteArrayId = null;
                this.exceptionMessage = null;
            }
            //clean the lock information
            LockOwner = null;
            LockExpirationTime = null;

            if (exceptionByteArrayIdToDelete != null)
            {
                //ByteArrayEntity byteArray = commandContext.GetDbEntityManager<ByteArrayEntity>().getDbEntityManager().selectById(ByteArrayEntity.class, exceptionByteArrayIdToDelete);
                var byteArray = commandContext.GetDbEntityManager<ResourceEntity>().Get(exceptionByteArrayIdToDelete);
                commandContext.GetDbEntityManager<ResourceEntity>().Delete(byteArray);
            }
        }

        public override string ToString()
        {
            return this.GetType().Name
                   + "[id=" + Id
                   + ", revision=" + Revision
                   + ", duedate=" + Duedate
                   + ", lockOwner=" + LockOwner
                   + ", lockExpirationTime=" + LockExpirationTime
                   + ", executionId=" + ExecutionId
                   + ", processInstanceId=" + ProcessInstanceId
                   + ", isExclusive=" + Exclusive
                   + ", retries=" + Retries
                   + ", jobHandlerType=" + JobHandlerType
                   + ", jobHandlerConfiguration=" + JobHandlerConfiguration
                   + ", exceptionByteArray=" + ExceptionByteArray
                   + ", exceptionByteArrayId=" + ExceptionByteArrayId
                   + ", exceptionMessage=" + ExceptionMessage
                   + ", deploymentId=" + DeploymentId
                   + "]";
        }

    }
}

