


using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{



    /// <summary>
    /// NOTE: instances of Messge Entity should be created via <seealso cref="MessageJobDeclaration"/>.
    /// 
    /// </summary>
    public class MessageEntity : JobEntity
    {

        public const string TYPE = "message";

        private const long SerialVersionUid = 1L;

        public override string Repeat { get; set; } = null;

        public override string Type => TYPE;

        public override string ToString()
        {
            return this.GetType().Name + "[repeat=" + Repeat + ", id=" + Id + ", revision=" + Revision + ", duedate=" + Duedate + ", lockOwner=" + LockOwner + ", lockExpirationTime=" + LockExpirationTime + ", executionId=" + ExecutionId + ", processInstanceId=" + ProcessInstanceId + ", isExclusive=" + Exclusive + ", retries=" + retries + ", jobHandlerType=" + JobHandlerType + ", jobHandlerConfiguration=" + JobHandlerConfigurationRaw + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + ExceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + DeploymentId + "]";
        }

    }

}