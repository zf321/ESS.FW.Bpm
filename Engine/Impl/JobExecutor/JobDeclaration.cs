using System;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    
    /// <summary>
    /// job定义，job实例工厂。
    ///     <para>
    ///         A job declaration is associated with an activity in the process definition graph.
    ///         It provides data about jobs which are to be created when executing this activity.
    ///         It also acts as a factory for new Job Instances.
    ///     </para>
    ///     <para>
    ///         Jobs are of a type T and are created in the context of type S (e.g. an execution or an event subscription).
    ///         An instance of the context class is handed in when a job is created.
    ///     </para>
    ///     
    /// </summary>
    [Serializable]
    public abstract class JobDeclaration<T> : IJobDeclaration where T : JobEntity
    {
        private const long SerialVersionUid = 1L;

        protected internal ActivityImpl activity;

        protected internal bool exclusive = JobEntity.DefaultExclusive;
        protected internal string jobConfiguration;

        /// <summary>
        ///     the id of the associated persistent jobDefinitionId
        /// </summary>
        protected internal string jobDefinitionId;

        protected internal IJobHandlerConfiguration jobHandlerConfiguration;

        protected internal string jobHandlerType;

        protected internal IParameterValueProvider jobPriorityProvider;

        public JobDeclaration(string jobHandlerType)
        {
            this.jobHandlerType = jobHandlerType;
        }

        // Getter / Setters //////////////////////////////////////////

        public virtual string JobDefinitionId
        {
            get { return jobDefinitionId; }
            set { jobDefinitionId = value; }
        }


        public virtual string JobHandlerType
        {
            get { return jobHandlerType; }
        }

        public virtual bool Exclusive
        {
            get { return exclusive; }
            set { exclusive = value; }
        }


        public virtual string ActivityId
        {
            get
            {
                if (activity != null)
                    return activity.Id;
                return null;
            }
        }

        public virtual ActivityImpl Activity
        {
            get { return activity; }
            set { activity = value; }
        }


        public virtual ProcessDefinitionImpl ProcessDefinition
        {
            get
            {
                if (activity != null)
                    return (ProcessDefinitionImpl)activity.ProcessDefinition;
                return null;
            }
        }

        public virtual string JobConfiguration
        {
            get { return jobConfiguration; }
            set { jobConfiguration = value; }
        }


        public virtual IParameterValueProvider JobPriorityProvider
        {
            get { return jobPriorityProvider; }
            set { jobPriorityProvider = value; }
        }

        // Job instance factory //////////////////////////////////////////

        /// <returns> the created Job instances </returns>
        public virtual T CreateJobInstance(object context)
        {
            var job = NewJobInstance(context);

            // set job definition id
            var jobDefinitionId = ResolveJobDefinitionId(context);
            job.JobDefinitionId = jobDefinitionId;

            if (!ReferenceEquals(jobDefinitionId, null))
            {
                JobDefinitionEntity jobDefinition = Context.CommandContext.JobDefinitionManager.FindById(jobDefinitionId);
                if (jobDefinition != null)
                {
                    // if job definition is suspended while creating a job instance,
                    // suspend the job instance right away:
                    job.SuspensionState = jobDefinition.SuspensionState;
                    job.ProcessDefinitionKey = jobDefinition.ProcessDefinitionKey;
                    job.ProcessDefinitionId = jobDefinition.ProcessDefinitionId;
                    job.TenantId = jobDefinition.TenantId;
                }
            }

            job.JobHandlerConfiguration = ResolveJobHandlerConfiguration(context);
            job.JobHandlerType = ResolveJobHandlerType(context);
            job.Exclusive = ResolveExclusive(context);
            job.Retries = ResolveRetries(context);
            job.Duedate = ResolveDueDate(context);


            // contentExecution can be null in case of a timer start event or
            // and batch jobs unrelated to executions
            var contextExecution = ResolveExecution(context);

            if (Context.ProcessEngineConfiguration.ProducePrioritizedJobs)
            {
                //TODO this:AsyncBeforeMessageJobDeclaration类型转换异常
                long priority = Context.ProcessEngineConfiguration.JobPriorityProvider.DeterminePriority(contextExecution, this, jobDefinitionId);
                
                job.Priority = priority;
            }

            if (contextExecution != null)
                job.TenantId = contextExecution.TenantId;

            PostInitialize(context, job);

            return job;
        }

        /// <summary>
        ///     general callback to override any configuration after the defaults have been applied
        /// </summary>
        protected  virtual void PostInitialize(object context, T job)
        {
        }

        /// <summary>
        ///     Returns the execution in which context the job is created. The execution
        ///     is used to determine the job's priority based on a BPMN activity
        ///     the execution is currently executing. May be null.
        /// </summary>
        protected internal abstract ExecutionEntity ResolveExecution(object context);

        protected internal abstract T NewJobInstance(object context);

        protected internal virtual string ResolveJobDefinitionId(object context)
        {
            return jobDefinitionId;
        }

        protected IJobHandler ResolveJobHandler()
        {
            IJobHandler jobHandler = Context.ProcessEngineConfiguration.JobHandlers[jobHandlerType];
            EnsureUtil.EnsureNotNull("Cannot find job handler '" + jobHandlerType + "' from job '" + this + "'", "jobHandler", jobHandler);

            return jobHandler;
        }

        protected internal virtual string ResolveJobHandlerType(object context)
        {
            return jobHandlerType;
        }

        protected internal abstract IJobHandlerConfiguration ResolveJobHandlerConfiguration(object context);

        protected internal virtual bool ResolveExclusive(object context)
        {
            return exclusive;
            //return false;
        }

        protected internal virtual int ResolveRetries(object context)
        {
            return Engine.context.Impl.Context.ProcessEngineConfiguration.DefaultNumberOfRetries;
        }

        protected internal virtual DateTime? ResolveDueDate(object context)
        {
            ProcessEngineConfiguration processEngineConfiguration = Engine.context.Impl.Context.ProcessEngineConfiguration;
            if ((processEngineConfiguration != null) && processEngineConfiguration.JobExecutorAcquireByDueDate)
                return ClockUtil.CurrentTime;
            return null;
        }
    }

    public interface IJobDeclaration
    {


        bool Exclusive { get; set; }

        string ActivityId { get; }

        ActivityImpl Activity { get; set; }


        ProcessDefinitionImpl ProcessDefinition { get; }

        string JobConfiguration { get; set; }


        IParameterValueProvider JobPriorityProvider { get; set; }

        string JobDefinitionId { get; set; }
        string JobHandlerType { get; }

        //// Job instance factory //////////////////////////////////////////

        ///// <returns> the created Job instances </returns>
        //T CreateJobInstance(object context);

        ///// <summary>
        /////     general callback to override any configuration after the defaults have been applied
        ///// </summary>
        //void PostInitialize(object context, T job);

        ///// <summary>
        /////     Returns the execution in which context the job is created. The execution
        /////     is used to determine the job's priority based on a BPMN activity
        /////     the execution is currently executing. May be null.
        ///// </summary>
        //ExecutionEntity ResolveExecution(object context);

        ////T NewJobInstance(object context);

        //string ResolveJobDefinitionId(object context);

        //  IJobHandler<TS> ResolveJobHandler<TS>() where TS: IJobHandlerConfiguration
        //{
        //    //throw new NotImplementedException();
        //    Impl.JobExecutor.IJobHandler<T> jobHandler = Context.ProcessEngineConfiguration.JobHandlers[jobHandlerType];
        //    EnsureUtil.EnsureNotNull("Cannot find job handler '" + jobHandlerType + "' from job '" + this + "'", "jobHandler", jobHandler);

        //    return jobHandler;
        //}

        //string ResolveJobHandlerType(object context);

        //IJobHandlerConfiguration ResolveJobHandlerConfiguration(object context);

        //bool ResolveExclusive(object context);

        //int ResolveRetries(object context);

        //DateTime ResolveDueDate(object context);
    }
}