using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Variable;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// Stub of the common parts of a Job. You will normally work with a subclass of
	/// JobEntity, such as <seealso cref="TimerEntity"/> or <seealso cref="MessageEntity"/>.
	/// 
	/// </summary>
	[Serializable]
    public class JobEntity : IJob, IDbEntity, IHasDbRevision
    {

        private static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        public const bool DefaultExclusive = true;
        public const int DefaultRetries = 3;

        /// <summary>
        /// Note: <seealso cref="String#length()"/> counts Unicode supplementary
        /// characters twice, so for a String consisting only of those,
        /// the limit is effectively MAX_EXCEPTION_MESSAGE_LENGTH / 2
        /// </summary>
        public static int MaxExceptionMessageLength = 666;

        private const long SerialVersionUid = 1L;

        protected internal int retries = DefaultRetries;

        // entity is active by default

        protected ResourceEntity exceptionByteArray;

        protected internal string exceptionMessage;

        // runtime state /////////////////////////////
        protected internal string activityId;
        protected internal IJobDefinition jobDefinition;
        protected internal ExecutionEntity execution;

        // sequence counter //////////////////////////


        public virtual void Init(CommandContext commandContext)
        {
            // nothing to do
        }

        public virtual void Execute(CommandContext commandContext)
        {
            if (ExecutionId != null)
            {
                ExecutionEntity exec = Execution;
                EnsureUtil.EnsureNotNull("Cannot find execution with id '" + ExecutionId + "' referenced from job '" + this + "'", "execution", exec);
            }

            // initialize activity id
            GetActivityId();

            // increment sequence counter before job execution
            IncrementSequenceCounter();

            PreExecute(commandContext);
            IJobHandler jobHandler = JobHandler;
            IJobHandlerConfiguration configuration = JobHandlerConfiguration;
            EnsureUtil.EnsureNotNull("Cannot find job handler '" + JobHandlerType + "' from job '" + this + "'", "jobHandler", jobHandler);
            jobHandler.Execute(configuration, execution, commandContext, TenantId);
            PostExecute(commandContext);
        }

        protected internal virtual void PreExecute(CommandContext commandContext)
        {
            // nothing to do
        }

        protected internal virtual void PostExecute(CommandContext commandContext)
        {
            Log.DebugJobExecuted(this);
            Delete(true);
            commandContext.HistoricJobLogManager.FireJobSuccessfulEvent(this);
        }

        public virtual void Insert()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;

            // add link to execution and deployment
            ExecutionEntity execution = Execution;
            if (execution != null)
            {
                execution.AddJob(this);

                ProcessDefinitionImpl processDefinition = execution.ProcessDefinition;//.getProcessDefinition();
                this.DeploymentId = processDefinition.DeploymentId;
            }
            commandContext.JobManager.InsertJob(this);
        }

        public virtual void Delete()
        {
            Delete(false);
        }

        public virtual void Delete(bool incidentResolved)
        {
            //throw new NotImplementedException();
            CommandContext commandContext = Context.CommandContext;

            IncrementSequenceCounter();

            // clean additional data related to this job
            IJobHandler jobHandler = JobHandler;
            if (jobHandler != null)
            {
                jobHandler.OnDelete(JobHandlerConfiguration, this);
            }

            // fire delete event if this job is not being executed
            bool executingJob = this.Equals(commandContext.CurrentJob);
            commandContext.JobManager.DeleteJob(this, !executingJob);

            // Also delete the job's exception byte array
            if (ExceptionByteArrayId != null)
            {
                commandContext.ByteArrayManager.DeleteByteArrayById(ExceptionByteArrayId);
            }

            // remove link to execution
            ExecutionEntity execution = Execution;
            if (execution != null)
            {
                execution.RemoveJob(this);
            }

            RemoveFailedJobIncident(incidentResolved);
        }

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["executionId"] = ExecutionId;
            persistentState["lockOwner"] = LockOwner;
            persistentState["lockExpirationTime"] = LockExpirationTime;
            persistentState["retries"] = retries;
            persistentState["duedate"] = Duedate;
            persistentState["exceptionMessage"] = exceptionMessage;
            persistentState["suspensionState"] = SuspensionState;
            persistentState["processDefinitionId"] = ProcessDefinitionId;
            persistentState["jobDefinitionId"] = JobDefinitionId;
            persistentState["deploymentId"] = DeploymentId;
            persistentState["jobHandlerConfiguration"] = JobHandlerConfigurationRaw;
            persistentState["priority"] = Priority;
            persistentState["tenantId"] = TenantId;
            if (ExceptionByteArrayId != null)
            {
                persistentState["exceptionByteArrayId"] = ExceptionByteArrayId;
            }
            return persistentState;
        }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        [NotMapped]
        public virtual ExecutionEntity Execution
        {
            set
            {
                if (value != null)
                {
                    this.execution = value;
                    ExecutionId = value.Id;
                    ProcessInstanceId = value.ProcessInstanceId;
                    this.execution.AddJob(this);
                }
                else
                {
                    this.execution.RemoveJob(this);
                    this.execution = value;
                    ProcessInstanceId = null;
                    ExecutionId = null;
                }
            }
            get
            {
                EnsureExecutionInitialized();
                return execution;
            }
        }

        // sequence counter /////////////////////////////////////////////////////////

        public virtual long SequenceCounter { get; set; } = 1;


        public virtual void IncrementSequenceCounter()
        {
            SequenceCounter++;
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual string ExecutionId { get; set; } = null;


        protected internal virtual void EnsureExecutionInitialized()
        {
            if (execution == null && ExecutionId != null)
            {
                execution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
            }
        }
        [NotMapped]
        public virtual int Retries
        {
            get
            {
                return retries;
            }
            set
            {
                // if value should be set to a negative value set it to 0
                if (value < 0)
                {
                    value = 0;
                }

                // Assuming: if the number of value will
                // be changed from 0 to x (x >= 1), means
                // that the corresponding incident is resolved.
                if (this.retries == 0 && value > 0)
                {
                    RemoveFailedJobIncident(true);
                }

                // If the value will be set to 0, an
                // incident has to be created.
                if (value == 0 && this.retries > 0)
                {
                    CreateFailedJobIncident();
                }
                this.retries = value;
            }
        }


        // special setter for MyBatis which does not influence incidents
        public virtual int RetriesFromPersistence
        {
            get
            {
                return retries;
            }
            set
            {
                this.retries = value;
            }
        }

        protected internal virtual void CreateFailedJobIncident()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.GetProcessEngineConfiguration();
            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;

            if (processEngineConfiguration.CreateIncidentOnFailedJobEnabled)
            {

                string incidentHandlerType = IncidentEntity.FailedJobHandlerType;

                // make sure job has an ID set:
                if (Id == null)
                {
                    Id = processEngineConfiguration.IdGenerator.NewGuid();//.NextId;

                }
                else
                {
                    // check whether there exists already an incident
                    // for this job
                    IList<IIncident> failedJobIncidents = context.Impl.Context.CommandContext.IncidentManager.FindIncidentByConfigurationAndIncidentType(Id, incidentHandlerType);

                    if (failedJobIncidents.Count > 0)
                    {
                        return;
                    }

                }

                IncidentContext incidentContext = CreateIncidentContext();
                incidentContext.ActivityId = ActivityId;

                processEngineConfiguration.getIncidentHandler(incidentHandlerType).HandleIncident(incidentContext, exceptionMessage);

            }
        }

        protected internal virtual void RemoveFailedJobIncident(bool incidentResolved)
        {
            IIncidentHandler handler = context.Impl.Context.ProcessEngineConfiguration.getIncidentHandler(IncidentEntity.FailedJobHandlerType);

            IncidentContext incidentContext = CreateIncidentContext();

            if (incidentResolved)
            {
                handler.ResolveIncident(incidentContext);
            }
            else
            {
                handler.DeleteIncident(incidentContext);
            }
        }

        protected internal virtual IncidentContext CreateIncidentContext()
        {
            IncidentContext incidentContext = new IncidentContext();
            incidentContext.ProcessDefinitionId = ProcessDefinitionId;
            incidentContext.ExecutionId = ExecutionId;
            incidentContext.TenantId = TenantId;
            incidentContext.Configuration = Id;
            incidentContext.JobDefinitionId = JobDefinitionId;

            return incidentContext;
        }
        [NotMapped]
        public virtual string ExceptionStacktrace
        {
            get
            {
                ResourceEntity byteArray = ExceptionByteArray;
                return ExceptionUtil.GetExceptionStacktrace(byteArray);
            }
            set
            {
                byte[] exceptionBytes = System.Text.Encoding.UTF8.GetBytes(value);// toByteArray(value);

                ResourceEntity byteArray = ExceptionByteArray;

                if (byteArray == null)
                {
                    byteArray = ExceptionUtil.CreateJobExceptionByteArray(exceptionBytes);
                    ExceptionByteArrayId = byteArray.Id;
                    exceptionByteArray = byteArray;
                }
                else
                {
                    byteArray.Bytes = exceptionBytes;
                }
            }
        }

        public virtual int SuspensionState { set; get; } = SuspensionStateFields.Active.StateCode;


        public virtual bool Suspended => SuspensionState == SuspensionStateFields.Suspended.StateCode;

        public virtual string LockOwner { get; set; } = null;


        public virtual DateTime? LockExpirationTime { get; set; } = null;


        public virtual string ProcessInstanceId { get; set; } = null;


        public virtual string ProcessDefinitionId { get; set; } = null;


        public virtual string ProcessDefinitionKey { get; set; } = null;

        public virtual bool Exclusive { get; set; } = DefaultExclusive;


        public virtual string Id { get; set; }


        public virtual DateTime? Duedate { get; set; }


        [NotMapped]
        protected internal virtual IJobHandler JobHandler
        {
            get
            {
                IDictionary<string, IJobHandler> jobHandlers = context.Impl.Context.ProcessEngineConfiguration.jobHandlers;
                return jobHandlers.GetValueOrNull(JobHandlerType);
                //if (!jobHandlers.ContainsKey(JobHandlerType))
                //{
                //    Log.LogDebug($"JobHandler  key:{JobHandlerType}", $"{string.Join(",", jobHandlers.Keys)}");
                //    throw new NotImplementedException(string.Format("引擎初始化配置中不包含key:{0}的jobHandler", JobHandlerType));
                //}
                //return jobHandlers[JobHandlerType];
            }
        }

        public virtual IJobHandlerConfiguration JobHandlerConfiguration
        {
            get
            {
                //key:tweet，初始化配置JobHandler中不包含该key
                //ENGINE-03JobHandler  key:tweet timer-transition,timer-intermediate-transition,timer-start-event,timer-start-event-subprocess,async-continuation,event,suspend-processdefinition,activate-processdefinition,suspend-job-definition,activate-job-definition,batch-seed-job,batch-monitor-job,instance-migration,instance-deletion,historic-instance-deletion,set-job-retries
                return JobHandler.NewConfiguration(JobHandlerConfigurationRaw);
            }
            set
            {
                this.JobHandlerConfigurationRaw = value.ToCanonicalString();
            }
        }


        public virtual string JobHandlerType { get; set; } = null;


        public virtual string JobHandlerConfigurationRaw { get; set; } = null;


        public virtual int Revision { get; set; }


        public virtual string ExceptionMessage
        {
            get
            {
                return exceptionMessage;
            }
            set
            {
                if (value != null && value.Length > MaxExceptionMessageLength)
                {
                    this.exceptionMessage = value.Substring(0, MaxExceptionMessageLength);
                }
                else
                {
                    this.exceptionMessage = value;
                }
            }
        }

        public virtual string JobDefinitionId { get; set; }


        public virtual IJobDefinition JobDefinition
        {
            get
            {
                EnsureJobDefinitionInitialized();
                return jobDefinition;
            }
            set
            {
                this.jobDefinition = value;
                if (value != null)
                {
                    JobDefinitionId = value.Id;
                }
                else
                {
                    JobDefinitionId = null;
                }
            }
        }


        protected internal virtual void EnsureJobDefinitionInitialized()
        {
            if (jobDefinition == null && JobDefinitionId != null)
            {
                jobDefinition = context.Impl.Context.CommandContext.JobDefinitionManager.FindById(JobDefinitionId);//.FindById(jobDefinitionId);
            }
        }

        public virtual string ExceptionByteArrayId { get; set; }

        protected internal virtual ResourceEntity ExceptionByteArray
        {
            get
            {
                EnsureExceptionByteArrayInitialized();
                return exceptionByteArray;
            }
        }



        protected internal virtual void EnsureExceptionByteArrayInitialized()
        {
            if (exceptionByteArray == null && ExceptionByteArrayId != null)
            {
                //exceptionByteArray = context.Impl.Context.CommandContext.DbEntityManager.SelectById<ResourceEntity>(exceptionByteArrayId);
                exceptionByteArray = context.Impl.Context.CommandContext.GetDbEntityManager<ResourceEntity>().Get(ExceptionByteArrayId);
            }
        }

        public virtual string DeploymentId { get; set; }


        public virtual bool InInconsistentLockState
        {
            get
            {
                return (LockOwner != null && LockExpirationTime == null) || (retries == 0 && (LockOwner != null || LockExpirationTime != null));
            }
        }

        public virtual void ResetLock()
        {
            this.LockOwner = null;
            this.LockExpirationTime = null;
        }

        public string GetActivityId()
        {
            EnsureActivityIdInitialized();
            return activityId;

        }
        [NotMapped]
        public virtual string ActivityId
        {
            get
            {
                EnsureActivityIdInitialized();
                return activityId;
            }
            set
            {
                this.activityId = value;
            }
        }


        public virtual long Priority { get; set; } = DefaultJobPriorityProvider.DEFAULT_PRIORITY;


        public virtual string TenantId { get; set; }


        protected internal virtual void EnsureActivityIdInitialized()
        {
            if (activityId == null)
            {
                IJobDefinition jobDefinition = JobDefinition;
                if (jobDefinition != null)
                {
                    activityId = jobDefinition.ActivityId;
                }
                else
                {
                    ExecutionEntity execution = Execution;
                    if (execution != null)
                    {
                        activityId = execution.ActivityId;
                    }
                }
            }
        }

        /// 
        /// <summary>
        /// Unlock from current lock owner
        /// 
        /// </summary>

        public virtual void Unlock()
        {
            this.LockOwner = null;
            this.LockExpirationTime = null;
        }
        //[NotMapped]
        public virtual string Type { get; }
        public virtual string Repeat { get; set; }
        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((Id == null) ? 0 : Id.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            JobEntity other = (JobEntity)obj;
            if (Id == null)
            {
                if (other.Id != null)
                {
                    return false;
                }
            }
            else if (!Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", duedate=" + Duedate + ", lockOwner=" + LockOwner + ", lockExpirationTime=" + LockExpirationTime + ", executionId=" + ExecutionId + ", processInstanceId=" + ProcessInstanceId + ", isExclusive=" + Exclusive + ", isExclusive=" + Exclusive + ", jobDefinitionId=" + JobDefinitionId + ", jobHandlerType=" + JobHandlerType + ", jobHandlerConfiguration=" + JobHandlerConfigurationRaw + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + ExceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + DeploymentId + ", priority=" + Priority + ", tenantId=" + TenantId + "]";
        }
    }

}