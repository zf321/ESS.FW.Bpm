using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Variable;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Engine.Variable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;
using ESS.FW.Bpm.Model.Xml.type;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExecutionEntity : PvmExecutionImpl, IProcessInstance, IDbEntity, IHasDbRevision, IHasDbReferences, IVariablesProvider
    {
        private bool _instanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            variableStore = new VariableStore(this, new ExecutionEntityReferencer(this));
        }


        private const long SerialVersionUid = 1L;

        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        // Persistent refrenced entities state //////////////////////////////////////
        public const int EventSubscriptionsStateBit = 1;
        public const int TasksStateBit = 2;
        public const int JobsStateBit = 3;
        public const int IncidentStateBit = 4;
        public const int VariablesStateBit = 5;
        public const int SubProcessInstanceStateBit = 6;
        public const int SubCaseInstanceStateBit = 7;
        public const int ExternalTasksBit = 8;

        // current position /////////////////////////////////////////////////////////

        /// <summary>
        /// the process instance. this is the root of the execution tree. the
        /// processInstance of a process instance is a self reference.
        /// </summary>
        [NonSerialized]
        protected internal ExecutionEntity processInstance;

        /// <summary>
        /// the parent execution </summary>
        protected internal ExecutionEntity parent;

        /// <summary>
        /// nested executions representing scopes or concurrent paths </summary>
        protected internal IList<IActivityExecution> executions;

        /// <summary>
        /// super execution, not-null if this execution is part of a subprocess </summary>


        /// <summary>
        /// super case execution, not-null if this execution is part of a case
        /// execution
        /// </summary>
        //protected internal CaseExecutionEntity SuperCaseExecution;

        /// <summary>
        /// reference to a subprocessinstance, not-null if currently subprocess is
        /// started from this execution
        /// </summary>
        protected internal ExecutionEntity subProcessInstance;

        public void SetExecutions(List<IActivityExecution> list)
        {
            this.executions = list;
        }
        internal void SetProcessDefinition(ProcessDefinitionImpl processDefinition)
        {
            this.processDefinition = processDefinition;
            this.ProcessDefinitionId = processDefinition.Id;
        }

        internal void SetProcessInstance(PvmExecutionImpl processInstance)
        {
            this.processInstance = processInstance as ExecutionEntity;

            if (processInstance != null)
            {
                this.ProcessInstanceId = this.processInstance.Id;
            }
        }

        /// <summary>
        /// reference to a subcaseinstance, not-null if currently subcase is started
        /// from this execution
        ///// </summary>
        //[NonSerialized]
        //protected internal CaseExecutionEntity SubCaseInstance;

        protected internal bool ShouldQueryForSubprocessInstance = false;

        protected internal bool ShouldQueryForSubCaseInstance = false;

        // associated entities /////////////////////////////////////////////////////

        // (we cache associated entities here to minimize db queries)
        [NonSerialized]
        [NotMapped]
        protected internal IList<EventSubscriptionEntity> eventSubscriptions;
        [NonSerialized]
        [NotMapped]
        protected internal IList<JobEntity> jobs;
        [NonSerialized]
        [NotMapped]
        protected internal IList<TaskEntity> tasks;
        [NonSerialized]
        [NotMapped]
        protected internal IList<ExternalTaskEntity> externalTasks;
        [NonSerialized]
        [NotMapped]
        protected internal IList<IncidentEntity> incidents;
        protected internal int cachedEntityState;

        [NonSerialized]
        [NotMapped]
        //protected internal VariableStore<ICoreVariableInstance> variableStore;
        protected internal VariableStore variableStore;

        // replaced by //////////////////////////////////////////////////////////////

        // Persistence //////////////////////////////////////////////////////////////

        public virtual int Revision { get; set; } = 1;

        public virtual string SuperExecutionId { get;
            set; }

        public virtual string SuperCaseExecutionId { get; set; }

        public int SuspensionState { get; set; } = SuspensionStateFields.Active.StateCode;

        [JsonIgnore]
        public virtual int CachedEntityState
        {
            set
            {
                this.cachedEntityState = value;

                // Check for flags that are down. These lists can be safely initialized as
                // empty, preventing
                // additional queries that end up in an empty list anyway
                if (jobs == null && !BitMaskUtil.IsBitOn(value, JobsStateBit))
                {
                    jobs = new List<JobEntity>();
                }
                if (tasks == null && !BitMaskUtil.IsBitOn(value, TasksStateBit))
                {
                    tasks = new List<TaskEntity>();
                }
                if (eventSubscriptions == null && !BitMaskUtil.IsBitOn(value, EventSubscriptionsStateBit))
                {
                    eventSubscriptions = new List<EventSubscriptionEntity>();
                }
                if (incidents == null && !BitMaskUtil.IsBitOn(value, IncidentStateBit))
                {
                    incidents = new List<IncidentEntity>();
                }
                if (!variableStore.Initialized && !BitMaskUtil.IsBitOn(value, VariablesStateBit))
                {
                    variableStore.VariablesProvider = VariableCollectionProvider.EmptyVariables();
                    variableStore.ForceInitialization();
                }
                if (externalTasks == null && !BitMaskUtil.IsBitOn(value, ExternalTasksBit))
                {
                    externalTasks = new List<ExternalTaskEntity>();
                }
                ShouldQueryForSubprocessInstance = BitMaskUtil.IsBitOn(value, SubProcessInstanceStateBit);
                ShouldQueryForSubCaseInstance = BitMaskUtil.IsBitOn(value, SubCaseInstanceStateBit);
            }
            get
            {
                cachedEntityState = 0;

                // Only mark a flag as false when the list is not-null and empty. If null,
                // we can't be sure there are no entries in it since
                // the list hasn't been initialized/queried yet.
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, TasksStateBit, (tasks == null || tasks.Count > 0));
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, EventSubscriptionsStateBit, (eventSubscriptions == null || eventSubscriptions.Count > 0));
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, JobsStateBit, (jobs == null || jobs.Count > 0));
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, IncidentStateBit, (incidents == null || incidents.Count > 0));
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, VariablesStateBit, (!variableStore.Initialized || !variableStore.Empty));
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, SubProcessInstanceStateBit, ShouldQueryForSubprocessInstance);
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, SubCaseInstanceStateBit, ShouldQueryForSubCaseInstance);
                cachedEntityState = BitMaskUtil.SetBit(cachedEntityState, ExternalTasksBit, (externalTasks == null || externalTasks.Count > 0));

                return cachedEntityState;
            }
        }

        /// <summary>
        /// The name of the current activity position
        /// </summary>
        protected internal string ActivityName;

        /// <summary>
        /// Contains observers which are observe the execution.
        /// 
        /// </summary>
        [JsonIgnore]
        protected internal IList<IExecutionObserver> ExecutionObservers = new List<IExecutionObserver>();

        [JsonIgnore]
        [NotMapped]
        protected internal IList<IVariableInstanceLifecycleListener<VariableInstanceEntity>> RegisteredVariableListeners = new List<IVariableInstanceLifecycleListener<VariableInstanceEntity>>();
        private ProcessDefinitionImpl processDefinition;

        public ExecutionEntity()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public new ExecutionEntity CreateExecution()
        {
            //throw new NotImplementedException();
            return ACreateExecution(false);
        }

        /// <summary>
        /// creates a new execution. properties processDefinition, processInstance and
        /// activity will be initialized.
        /// </summary>
        public ExecutionEntity ACreateExecution(bool initializeExecutionStartContext)
        {
            // create the new child execution
            ExecutionEntity createdExecution = CreateNewExecution();

            // initialize sequence counter
            createdExecution.SequenceCounter = SequenceCounter;

            // manage the bidirectional parent-child relation
            createdExecution.SetParent(this);//.Parent = this;

            // initialize the new execution
            createdExecution.SetProcessDefinition(GetProcessDefinition());
            createdExecution.SetProcessInstance(GetProcessInstance());
            createdExecution.SetActivity(GetActivity());
            createdExecution.SuspensionState = SuspensionState;

            // make created execution start in same activity instance
            createdExecution.ActivityInstanceId = ActivityInstanceId;

            // inherit the tenant id from parent execution
            if (TenantId != null)
            {
                createdExecution.TenantId = TenantId;
            }

            if (initializeExecutionStartContext)
            {
                createdExecution.StartContext = new ExecutionStartContext();
            }
            else if (StartContext != null)
            {
                createdExecution.StartContext = StartContext;
            }

            createdExecution.SkipCustomListeners = this.SkipCustomListeners;
            createdExecution.SkipIoMapping = this.SkipIoMapping;

            Log.CreateChildExecution(createdExecution, this);

            return createdExecution;
        }

        // sub process instance
        // /////////////////////////////////////////////////////////////

        public override IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition, string businessKey, string caseInstanceId)
        {
            ShouldQueryForSubprocessInstance = true;

            ExecutionEntity subProcessInstance = (ExecutionEntity)base.CreateSubProcessInstance(processDefinition, businessKey, caseInstanceId);

            // inherit the tenant-id from the process definition
            string tenantId = ((ProcessDefinitionEntity)processDefinition).TenantId;
            if (tenantId != null)
            {
                subProcessInstance.TenantId = tenantId;
            }
            else
            {
                // if process definition has no tenant id, inherit this process instance's tenant id
                subProcessInstance.TenantId = this.TenantId;
            }

            FireHistoricActivityInstanceUpdate();

            return subProcessInstance;
        }

        protected internal static ExecutionEntity CreateNewExecution()
        {
            ExecutionEntity newExecution = new ExecutionEntity();
            InitializeAssociations(newExecution);
            newExecution.Insert();

            return newExecution;
        }

        protected internal override PvmExecutionImpl NewExecution()
        {
            return CreateNewExecution();
        }

        // sub case instance ////////////////////////////////////////////////////////

        //public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition)
        //{
        //    return createSubCaseInstance(caseDefinition, null);
        //}

        //public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey)
        //{
        //    CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)caseDefinition.createCaseInstance(businessKey);

        //    // inherit the tenant-id from the case definition
        //    string tenantId = ((CaseDefinitionEntity)caseDefinition).TenantId;
        //    if (tenantId != null)
        //    {
        //        subCaseInstance.TenantId = tenantId;
        //    }
        //    else
        //    {
        //        // if case definition has no tenant id, inherit this process instance's tenant id
        //        subCaseInstance.TenantId = this.tenantId;
        //    }

        //    // manage bidirectional super-process-sub-case-instances relation
        //    subCaseInstance.SuperExecution = this;
        //    setSubCaseInstance(subCaseInstance);

        //    FireHistoricActivityInstanceUpdate();

        //    return subCaseInstance;
        //}

        // helper ///////////////////////////////////////////////////////////////////

        public virtual void FireHistoricActivityInstanceUpdate()
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;
            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceUpdate, this))
            {
                // publish update event for current activity instance (containing the id
                // of the sub process/case)
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly ExecutionEntity _outerInstance;

            public HistoryEventCreatorAnonymousInnerClassHelper(ExecutionEntity outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateActivityInstanceUpdateEvt(_outerInstance);
            }
        }

        // scopes ///////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            Log.InitializeExecution(this);

            ScopeImpl scope = ScopeActivity;
            EnsureParentInitialized();

            IList<VariableDeclaration> variableDeclarations = (IList<VariableDeclaration>)scope.GetProperty(BpmnParse.PropertynameVariableDeclarations);
            if (variableDeclarations != null)
            {
                foreach (VariableDeclaration variableDeclaration in variableDeclarations)
                {

                    variableDeclaration.Initialize(this, parent);
                }
            }

            if (IsProcessInstanceExecution)
            {
                string initiatorVariableName = (string)ProcessDefinition.GetProperty(BpmnParse.PropertynameInitiatorVariableName);
                if (initiatorVariableName != null)
                {
                    //TODO
                    string authenticatedUserId = context.Impl.Context.CommandContext.AuthenticatedUserId;
                    SetVariable(initiatorVariableName, authenticatedUserId);
                }
            }

            // create event subscriptions for the current scope
            foreach (EventSubscriptionDeclaration declaration in EventSubscriptionDeclaration.GetDeclarationsForScope(scope).Values)
            {
                if (!declaration.StartEvent)
                {

                    declaration.CreateSubscriptionForExecution(this);
                }
            }
        }

        public override void InitializeTimerDeclarations()
        {

            Log.InitializeTimerDeclaration(this);
            ScopeImpl scope = ScopeActivity;
            ICollection<TimerDeclarationImpl> timerDeclarations = TimerDeclarationImpl.GetDeclarationsForScope(scope).Values;
            foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
            {
                timerDeclaration.CreateTimerInstance(this);
            }
        }

        protected internal static void InitializeAssociations(ExecutionEntity execution)
        {
            //initialize the lists of referenced objects(prevents db queries)
            execution.executions = new List<IActivityExecution>();
            execution.variableStore.VariablesProvider = VariableCollectionProvider.EmptyVariables();
            execution.variableStore.ForceInitialization();
            execution.eventSubscriptions = new List<EventSubscriptionEntity>();
            execution.jobs = new List<JobEntity>();
            execution.tasks = new List<TaskEntity>();
            execution.externalTasks = new List<ExternalTaskEntity>();
            execution.incidents = new List<IncidentEntity>();

            // Cached entity-state initialized to null, all bits are zero, indicating NO
            // entities present
            execution.cachedEntityState = 0;

        }

        public override void Start(IDictionary<string, object> variables)
        {
            // determine tenant Id if null
            ProvideTenantId(variables);
            base.Start(variables);
        }

        public override void StartWithoutExecuting(IDictionary<string, object> variables)
        {
            ProvideTenantId(variables);
            base.StartWithoutExecuting(variables);
        }

        protected internal virtual void ProvideTenantId(IDictionary<string, object> variables)
        {

            if (TenantId == null)
            {
                ITenantIdProvider tenantIdProvider = Context.ProcessEngineConfiguration.TenantIdProvider;

                if (tenantIdProvider != null)
                {
                    //IVariableMap variableMap = Variables.FromMap(variables);
                    IVariableMap variableMap = Bpm.Engine.Variable.Variables.FromMap(variables);
                    IProcessDefinition processDefinition = (IProcessDefinition)GetProcessDefinition();

                    TenantIdProviderProcessInstanceContext ctx;
                    if (SuperExecutionId != null)
                    {
                        ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap, SuperExecution);
                    }
                    //else if (SuperCaseExecutionId != null)
                    //{

                    //    //ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap, SuperCaseExecution);
                    //}
                    else
                    {
                        ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap);
                    }

                    TenantId = tenantIdProvider.ProvideTenantIdForProcessInstance(ctx);
                }
            }
        }

        public virtual void StartWithFormProperties(IVariableMap properties)
        {

            ProvideTenantId(properties);
            if (IsProcessInstanceExecution)
            {
                ActivityImpl initial = ProcessDefinition.Initial as ActivityImpl;
                ProcessInstanceStartContext processInstanceStartContext = ProcessInstanceStartContext;
                if (processInstanceStartContext != null)
                {
                    initial = processInstanceStartContext.Initial;
                }
                FormPropertyStartContext formPropertyStartContext = new FormPropertyStartContext(initial);
                formPropertyStartContext.FormProperties = properties;
                StartContext = formPropertyStartContext;

                Initialize();
                InitializeTimerDeclarations();
                FireHistoricProcessStartEvent();
            }

            PerformOperation(PvmAtomicOperationFields.ProcessStart);
        }

        public override void FireHistoricProcessStartEvent()
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;
            IHistoryLevel historyLevel = configuration.HistoryLevel;
            // TODO: This smells bad, as the rest of the history is done via the
            // ParseListener
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceStart, ProcessInstance))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly ExecutionEntity _outerInstance;

            public HistoryEventCreatorAnonymousInnerClassHelper2(ExecutionEntity outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {

                return producer.CreateProcessInstanceStartEvt(_outerInstance.ProcessInstance);
            }
        }

        /// <summary>
        /// Method used for destroying a scope in a way that the execution can be
        /// removed afterwards.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            EnsureParentInitialized();

            // execute Output Mappings (if they exist).
            EnsureActivityInitialized();
            if (Activity != null && Activity.IoMapping != null && !SkipIoMapping)
            {
                Activity.IoMapping.ExecuteOutputParameters(this);
            }
            ClearExecution();
            RemoveEventSubscriptionsExceptCompensation();
        }

        protected internal virtual void ClearExecution()
        {

            //call the onRemove method of the execution observers
            //so they can do some clean up before
            foreach (IExecutionObserver observer in ExecutionObservers)
            {
                observer.OnClear(this);
            }

            // delete all the variable instances
            RemoveVariablesLocalInternal();

            // delete all the tasks
            RemoveTasks(null);

            // delete external tasks
            RemoveExternalTasks();

            // remove all jobs
            RemoveJobs();

            // remove all incidents
            RemoveIncidents();
        }

        protected internal override void RemoveVariablesLocalInternal()
        {
            foreach (VariableInstanceEntity variableInstance in variableStore.Variables)
            {
                base.InvokeVariableLifecycleListenersDelete(variableInstance, this, new List<IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>>() { VariablePersistenceListener });
                RemoveVariableInternal(variableInstance);
            }
        }

        public override void Interrupt(string reason, bool skipCustomListeners, bool skipIoMappings)
        {

            // remove Jobs
            if (PreserveScope)
            {
                RemoveActivityJobs(reason);
            }
            else
            {
                RemoveJobs();
                RemoveEventSubscriptionsExceptCompensation();
            }

            RemoveTasks(reason);

            base.Interrupt(reason, skipCustomListeners, skipIoMappings);
        }

        protected internal virtual void RemoveActivityJobs(string reason)
        {
            if (ActivityId != null)
            {
                foreach (JobEntity job in Jobs)
                {
                    if (ActivityId.Equals(job.ActivityId))
                    {
                        job.Delete();
                        RemoveJob(job);
                    }
                }

            }

        }

        // methods that translate to operations /////////////////////////////////////

        public override void PerformOperation<T>(ICoreAtomicOperation<T> operation)
        {
            if (operation is IAtomicOperation)
            {
                PerformOperation((IAtomicOperation)operation);
            }
            else
            {
                base.PerformOperation(operation);
            }
        }

        public override void PerformOperationSync<T>(ICoreAtomicOperation<T> operation)
        {
            if (operation is IAtomicOperation)
            {
                PerformOperationSync((IAtomicOperation)operation);
            }
            else
            {
                base.PerformOperationSync(operation);
            }
        }

        public virtual void PerformOperation(IAtomicOperation executionOperation)
        {
            bool @async = executionOperation.IsAsync(this);

            if (!@async && RequiresUnsuspendedExecution(executionOperation))
            {
                EnsureNotSuspended();
            }

            Context.CommandInvocationContext.PerformOperation(executionOperation, this, @async);
        }

        public virtual void PerformOperationSync(IAtomicOperation executionOperation)
        {
            if (RequiresUnsuspendedExecution(executionOperation))
            {
                EnsureNotSuspended();
            }

            Context.CommandInvocationContext.PerformOperation(executionOperation, this);
        }

        protected internal virtual void EnsureNotSuspended()
        {

            if (IsSuspended)
            {
                throw Log.SuspendedEntityException("Execution", Id);
            }
        }

        protected internal virtual bool RequiresUnsuspendedExecution(IAtomicOperation executionOperation)
        {
            if (executionOperation != PvmAtomicOperationFields.TransitionNotifyListenerEnd && executionOperation != PvmAtomicOperationFields.TransitionDestroyScope && executionOperation != PvmAtomicOperationFields.TransitionNotifyListenerTake && executionOperation != PvmAtomicOperationFields.TransitionNotifyListenerEnd && executionOperation != PvmAtomicOperationFields.TransitionCreateScope && executionOperation != PvmAtomicOperationFields.TransitionNotifyListenerStart && executionOperation !=
              PvmAtomicOperationFields.DeleteCascade && executionOperation !=
              PvmAtomicOperationFields.DeleteCascadeFireActivityEnd)
            {
                return true;
            }

            return false;
        }
        //异步方法 就是Context.CommandContext.JobManager.Send(MessageEntity) ?
        public virtual void ScheduleAtomicOperationAsync(AtomicOperationInvocation executionOperationInvocation)
        {

            MessageJobDeclaration messageJobDeclaration = null;

            IList<MessageJobDeclaration> messageJobDeclarations = (IList<MessageJobDeclaration>)Activity.GetProperty(BpmnParse.PropertynameMessageJobDeclaration);
            if (messageJobDeclarations != null)
            {
                foreach (MessageJobDeclaration declaration in messageJobDeclarations)
                {
                    if (declaration.IsApplicableForOperation(executionOperationInvocation.Operation))
                    {
                        messageJobDeclaration = declaration;
                        break;
                    }
                }
            }

            if (messageJobDeclaration != null)
            {

                MessageEntity message = messageJobDeclaration.CreateJobInstance(executionOperationInvocation);

                Context.CommandContext.JobManager.Send(message);//这就是异步执行？

            }
            else
            {
                throw Log.RequiredAsyncContinuationException(Activity.Id);
            }
        }


        // executions ///////////////////////////////////////////////////////////////

        public override void AddExecutionObserver(IExecutionObserver observer)
        {

            ExecutionObservers.Add(observer);
        }

        public virtual void RemoveExecutionObserver(IExecutionObserver observer)
        {

            ExecutionObservers.Remove(observer);
        }

        [JsonIgnore]
        public override IList<IActivityExecution> Executions
        {
            get
            {
                EnsureExecutionsInitialized();
                return executions;
            }
            //set
            //{
            //    this.executions = (IList<ExecutionEntity>) value;
            //}
        }
        public override IList<IActivityExecution> GetExecutionsEntity()
        {
            EnsureExecutionsInitialized();
            return executions;
        }

        [JsonIgnore]
        public override IList<IActivityExecution> ExecutionsAsCopy
        {
            get
            {
                return new List<IActivityExecution>(Executions);
            }
        }

        protected internal virtual void EnsureExecutionsInitialized()
        {
            if (executions == null)
            {
                if (ExecutionTreePrefetchEnabled)
                {
                    EnsureExecutionTreeInitialized();
                }
                else
                {
                    this.executions = (IList<IActivityExecution>)Context.CommandContext.ExecutionManager.FindChildExecutionsByParentExecutionId(Id);
                }

            }
        }

        ///// <returns> true if execution tree prefetching is enabled </returns>
        [JsonIgnore]
        protected internal virtual bool ExecutionTreePrefetchEnabled
        {
            get
            {
                return Context.ProcessEngineConfiguration.ExecutionTreePrefetchEnabled;
            }
        }


        //// bussiness key ////////////////////////////////////////////////////////////

        [JsonIgnore]
        public override string ProcessBusinessKey
        {
            get
            {
                if (ProcessInstance != null)
                {
                    return ProcessInstance.BusinessKey;
                }
                return BusinessKey;
            }
        }

        //// process definition ///////////////////////////////////////////////////////

        ///// <summary>
        ///// ensures initialization and returns the process definition. </summary>
        public ProcessDefinitionEntity GetProcessDefinition()
        {
            EnsureProcessDefinitionInitialized();
            return (ProcessDefinitionEntity)processDefinition;
        }
        /// <summary>
        /// persisted reference to the processDefinition.
        /// </summary>
        /// <seealso cref= #processDefinition </seealso>
        /// <seealso cref= #setProcessDefinition(ProcessDefinitionImpl) </seealso>
        /// <seealso cref= #getProcessDefinition() </seealso>
        public override string ProcessDefinitionId { get; set; }
        ///// <summary>
        ///// for setting the process definition, this setter must be used as subclasses
        ///// can override
        ///// </summary>
        protected internal virtual void EnsureProcessDefinitionInitialized()
        {
            if ((processDefinition == null) && (ProcessDefinitionId != null))
            {
                ProcessDefinitionEntity deployedProcessDefinition = Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);
                ProcessDefinition = deployedProcessDefinition;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public override ProcessDefinitionImpl ProcessDefinition
        {
            get
            {
                EnsureProcessDefinitionInitialized();
                return processDefinition;
            }
            set
            {
                this.processDefinition = value;
                this.ProcessDefinitionId = value.Id;
            }
        }

        //// process instance /////////////////////////////////////////////////////////

        ///// <summary>
        ///// ensures initialization and returns the process instance. </summary>
        [JsonIgnore]
        public override IDelegateExecution ProcessInstance
        {
            get
            {
                EnsureProcessInstanceInitialized();
                return processInstance;
            }
            set
            {
                this.processInstance = (ExecutionEntity)value;
                if (processInstance != null)
                {
                    this.ProcessInstanceId = this.processInstance.Id;
                }
            }
        }

        protected internal virtual void EnsureProcessInstanceInitialized()
        {
            if ((processInstance == null) && (ProcessInstanceId != null))
            {

                if (ExecutionTreePrefetchEnabled)
                {
                    EnsureExecutionTreeInitialized();

                }
                else
                {
                    processInstance = Context.CommandContext.ExecutionManager.FindExecutionById(ProcessInstanceId);
                }

            }
        }


        public override bool IsProcessInstanceExecution
        {
            get
            {
                return ParentId == null;
            }
        }

        //// Activity /////////////////////////////////////////////////////////////////
        private IPvmActivity _activity { get; set; }
        ///// <summary>
        ///// ensures initialization and returns the Activity </summary>
        [JsonIgnore]
        public override IPvmActivity Activity
        {
            get
            {
                EnsureActivityInitialized();
                return _activity;
            }
            set
            {
                _activity = value;
                if (value != null)
                {
                    this.ActivityId = value.Id;
                    this.ActivityName = (string)value.GetProperty("name");
                }
                else
                {
                    this.ActivityId = null;
                    this.ActivityName = null;
                }
            }
        }
        public IPvmActivity GetActivity()
        {
            EnsureActivityInitialized();
            return Activity;
        }
        public void SetActivity(IPvmActivity activity)
        {
            base.Activity = activity;
            if (activity != null)
            {
                this.ActivityId = activity.Id;
                this.ActivityName = (string)activity.GetProperty("name");
                this.Activity = activity;
            }
            else
            {
                this.ActivityId = null;
                this.ActivityName = null;
            }
        }
        /// <summary>
        /// persisted reference to the current position in the diagram within the
        /// <seealso cref="#processDefinition"/>.
        /// </summary>
        /// <seealso cref= #activity </seealso>
        /// <seealso cref= #getActivity() </seealso>
        public override string ActivityId { get; set; }


        /// <summary>
        /// must be called before the activity member field or getActivity() is called </summary>
        protected internal virtual void EnsureActivityInitialized()
        {
            if ((_activity == null) && (ActivityId != null))
            {
                Activity = (ActivityImpl)(GetProcessDefinition().FindActivity(ActivityId));
            }
        }


        /// <summary>
        /// generates an activity instance id
        /// </summary>
        protected internal override string GenerateActivityInstanceId(string activityId)
        {

            if (activityId.Equals(ProcessDefinitionId))
            {
                return ProcessInstanceId;

            }
            else
            {

                string nextId = context.Impl.Context.ProcessEngineConfiguration.IdGenerator.NewGuid();//.NextId;

                string compositeId = activityId + ":" + nextId;
                if (compositeId.Length > 64)
                {
                    return Convert.ToString(nextId);
                }
                else
                {
                    return compositeId;
                }
            }
        }

        // parent ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// ensures initialization and returns the parent </summary>
        [NotMapped]//[ForeignKey("ParentId")]
        [JsonIgnore]
        public override IActivityExecution Parent
        {
            get
            {

                EnsureParentInitialized();
                return parent;
            }
            set
            {
                SetParent(value);
            }

        }

        protected internal virtual void EnsureParentInitialized()
        {

            if (parent == null && ParentId != null)
            {
                if (ExecutionTreePrefetchEnabled)
                {
                    EnsureExecutionTreeInitialized();

                }
                else
                {
                    parent = Context.CommandContext.ExecutionManager.FindExecutionById(ParentId);
                }
            }
        }

        [NotMapped]
        [JsonIgnore]
        public override IActivityExecution ParentExecution
        {
            get { return Parent; }

            set
            {
                this.parent = (ExecutionEntity)value;

                if (value != null)
                {
                    this.ParentId = value.Id;
                }
                else
                {
                    this.ParentId = null;
                }
            }
        }

        // super- and subprocess executions /////////////////////////////////////////


        private ExecutionEntity _supeExecution;

        public override IDelegateExecution SuperExecution
        {
            get
            {

                EnsureSuperExecutionInitialized();
                return _supeExecution;
            }
            set
            {
                if (SuperExecutionId != null)
                {
                    EnsureSuperExecutionInitialized();
                    this._supeExecution.SubProcessInstance = (null);
                }

                this._supeExecution = value as ExecutionEntity;

                if (_supeExecution != null)
                {
                    this.SuperExecutionId = _supeExecution.Id;
                    (this._supeExecution).SubProcessInstance = (this);
                }
                else
                {
                    this.SuperExecutionId = null;
                }
            }
        }

        protected internal virtual void EnsureSuperExecutionInitialized()
        {
            if (_supeExecution == null && SuperExecutionId != null)
            {
                _supeExecution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(SuperExecutionId);
            }
        }

        [JsonIgnore]
        public override IActivityExecution SubProcessInstance
        {
            get
            {
                EnsureSubProcessInstanceInitialized();
                return subProcessInstance;
            }
            set
            {
                ShouldQueryForSubprocessInstance = value != null;
                this.subProcessInstance = (ExecutionEntity)value;
            }
        }
        public ExecutionEntity GetSubProcessInstance()
        {
            EnsureSubProcessInstanceInitialized();
            return subProcessInstance;
        }

        protected internal virtual void EnsureSubProcessInstanceInitialized()
        {
            if (ShouldQueryForSubprocessInstance && subProcessInstance == null)
            {
                subProcessInstance = context.Impl.Context.CommandContext.ExecutionManager.FindSubProcessInstanceBySuperExecutionId(Id);
            }
        }

        // super case executions ///////////////////////////////////////////////////




        //public CaseExecutionEntity GetSuperCaseExecution()
        //{
        //    EnsureSuperCaseExecutionInitialized();
        //    return SuperCaseExecution;
        //}

        //public void SetSuperCaseExecution(CmmnExecution superCaseExecution)
        //{
        //    this.SuperCaseExecution = (CaseExecutionEntity)superCaseExecution;

        //    if (superCaseExecution != null)
        //    {
        //        this.superCaseExecutionId = superCaseExecution.Id;
        //        this.caseInstanceId = superCaseExecution.GetCaseInstanceId();
        //    }
        //    else
        //    {
        //        this.superCaseExecutionId = null;
        //        this.caseInstanceId = null;
        //    }
        //}

        protected internal virtual void EnsureSuperCaseExecutionInitialized()
        {
            //if (SuperCaseExecution == null && superCaseExecutionId != null)
            //{
            //    SuperCaseExecution = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(superCaseExecutionId);
            //}
        }

        // sub case execution //////////////////////////////////////////////////////

        //public  CaseExecutionEntity GetSubCaseInstance()
        //{
        //    EnsureSubCaseInstanceInitialized();
        //    return SubCaseInstance;

        //}

        //public  void SetSubCaseInstance(CmmnExecution subCaseInstance)
        //{
        //    ShouldQueryForSubCaseInstance = subCaseInstance != null;
        //    this.SubCaseInstance = (CaseExecutionEntity)subCaseInstance;
        //}

        protected internal virtual void EnsureSubCaseInstanceInitialized()
        {
            //if (ShouldQueryForSubCaseInstance && SubCaseInstance == null)
            //{
            //    SubCaseInstance = Context.CommandContext.CaseExecutionManager.findSubCaseInstanceBySuperExecutionId(id);
            //}
        }

        // customized persistence behavior /////////////////////////////////////////

        public override void Remove()
        {
            base.Remove();

            // removes jobs, incidents and tasks, and
            // clears the variable store
            ClearExecution();

            // remove all event subscriptions for this scope, if the scope has event
            // subscriptions:
            RemoveEventSubscriptions();

            // finally delete this execution
            context.Impl.Context.CommandContext.ExecutionManager.DeleteExecution(this);
        }

        protected internal virtual void RemoveEventSubscriptionsExceptCompensation()
        {
            // remove event subscriptions which are not compensate event subscriptions
            IList<EventSubscriptionEntity> _eventSubscriptions = EventSubscriptions.Where(m => m.EventType != EventType.Compensate.Name).ToList();
            //while (eventSubscriptions.Count > 0)
            //{
            //    eventSubscriptions[0].Delete();
            //}
            //foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
            //{
            //    if (!EventType.Compensate.Name.Equals(eventSubscriptionEntity.EventType))
            //    {
            //        eventSubscriptionEntity.Delete();
            //    }
            //}
            //int count = eventSubscriptions.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    eventSubscriptions[i].Delete();
            //}
            if (_eventSubscriptions.Count == 0)
                return;
            Queue<EventSubscriptionEntity> evs = new Queue<EventSubscriptionEntity>(_eventSubscriptions);
            while (evs.Count > 0)
            {
                var current =_eventSubscriptions.First(m=>m.ToString()== evs.Dequeue().ToString());
                eventSubscriptions.Remove(current);
            }
        }

        public virtual void RemoveEventSubscriptions()
        {
            //foreach (EventSubscriptionEntity eventSubscription in EventSubscriptions)
            //{
            //    if (ReplacedBy != null)
            //    {
            //        eventSubscription.SetExecution((ExecutionEntity) ReplacedBy);
            //    }
            //    else
            //    {
            //        eventSubscription.Delete();
            //    }
            //}
            //TODO for循环 可能有问题
            //int count = EventSubscriptions.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    if (ReplacedBy != null)
            //    {
            //        EventSubscriptions[i].SetExecution(ReplacedBy as ExecutionEntity);
            //    }
            //    else
            //    {
            //        eventSubscriptions.RemoveAt(i);
            //    }
            //}
            var eventSubscriptions = EventSubscriptions;
            Queue<EventSubscriptionEntity> queue = new Queue<EventSubscriptionEntity>(eventSubscriptions);
            while (queue.Count > 0)
            {
                EventSubscriptionEntity e = queue.Dequeue();
                var current = eventSubscriptions.First(m => m.ToString() == e.ToString());
                if (ReplacedBy != null)
                {
                    current.SetExecution((ExecutionEntity)ReplacedBy);
                }
                else
                {
                    eventSubscriptions.Remove(current);
                }
            }
        }

        private void RemoveJobs()
        {
            foreach (IJob job in Jobs)
            {
                if (ReplacedByParent)
                {
                    ((JobEntity)job).Execution = (ExecutionEntity)ReplacedBy;
                }
                else
                {
                    ((JobEntity)job).Delete();
                }
            }
        }

        private void RemoveIncidents()
        {
            foreach (IncidentEntity incident in Incidents)
            {
                if (ReplacedByParent)
                {
                    incident.Execution = (ExecutionEntity)ReplacedBy;
                }
                else
                {
                    incident.Delete();
                }
            }
        }

        protected internal virtual void RemoveTasks(string reason)
        {
            if (reason == null)
            {
                reason = TaskEntity.DeleteReasonDeleted;
            }
            foreach (TaskEntity task in Tasks)
            {
                if (ReplacedByParent)
                {
                    if (task.GetExecution() == null || task.GetExecution() != ReplacedBy)
                    {
                        // All tasks should have been moved when "ReplacedBy" has been set.
                        // Just in case tasks where added,
                        // wo do an additional check here and move it
                        task.SetExecution(ReplacedBy);
                        //this.ReplacedBy.AddTask(task);
                    }
                }
                else
                {
                    task.Delete(reason, false, SkipCustomListeners);
                }
            }
        }

        protected internal virtual void RemoveExternalTasks()
        {
            foreach (ExternalTaskEntity externalTask in ExternalTasks)
            {
                externalTask.Delete();
            }
        }

        public override void Replace(IActivityExecution execution)
        {
            ExecutionEntity replacedExecution = (ExecutionEntity)execution;

            ListenerIndex = replacedExecution.ListenerIndex;
            replacedExecution.ListenerIndex = 0;

            // update the related tasks
            replacedExecution.MoveTasksTo(this);

            replacedExecution.MoveExternalTasksTo(this);

            // update those jobs that are directly related to the argument execution's
            // current activity
            replacedExecution.MoveActivityLocalJobsTo(this);

            if (!replacedExecution.IsEnded)
            {
                // on compaction, move all variables
                if (replacedExecution.Parent == this)
                {
                    replacedExecution.MoveVariablesTo(this);
                }
                // on expansion, move only concurrent local variables
                else
                {
                    replacedExecution.MoveConcurrentLocalVariablesTo(this);
                }
            }

            // note: this method not move any event subscriptions since concurrent
            // executions
            // do not have event subscriptions (and either one of the executions
            // involved in this
            // operation is concurrent)

            base.Replace(replacedExecution);
        }

        public override void OnConcurrentExpand(PvmExecutionImpl scopeExecution)
        {
            ExecutionEntity scopeExecutionEntity = (ExecutionEntity)scopeExecution;
            scopeExecutionEntity.MoveConcurrentLocalVariablesTo(this);
            base.OnConcurrentExpand(scopeExecutionEntity);
        }

        protected internal virtual void MoveTasksTo(ExecutionEntity other)
        {
            CommandContext commandContext = Context.CommandContext;

            // update the related tasks
            foreach (TaskEntity task in TasksInternal)
            {
                task.SetExecution(other);

                // update the related local task variables
                ICollection<Engine.Impl.Core.Variable.ICoreVariableInstance> variables = task.VariablesInternal;

                foreach (VariableInstanceEntity variable in variables)
                {
                    variable.Execution = other;
                }

                other.AddTask(task);
            }
            TasksInternal.Clear();
        }

        protected internal virtual void MoveExternalTasksTo(ExecutionEntity other)
        {
            foreach (ExternalTaskEntity externalTask in ExternalTasksInternal)
            {
                externalTask.ExecutionId = other.Id;
                externalTask.SetExecution(other);

                other.AddExternalTask(externalTask);
            }

            ExternalTasksInternal.Clear();
        }

        protected internal virtual void MoveActivityLocalJobsTo(ExecutionEntity other)
        {
            if (ActivityId != null)
            {
                foreach (JobEntity job in Jobs)
                {

                    if (ActivityId.Equals(job.ActivityId))
                    {
                        RemoveJob(job);
                        job.Execution = other;
                    }
                }
            }
        }

        protected internal virtual void MoveVariablesTo(ExecutionEntity other)
        {
            IList<Engine.Impl.Core.Variable.ICoreVariableInstance> variables = variableStore.Variables;
            variableStore.RemoveVariables();

            foreach (VariableInstanceEntity variable in variables)
            {
                MoveVariableTo(variable, other);
            }
        }

        protected internal virtual void MoveVariableTo(VariableInstanceEntity variable, ExecutionEntity other)
        {
            if (other.variableStore.ContainsKey(variable.Name))
            {
                Engine.Impl.Core.Variable.ICoreVariableInstance existingInstance = other.variableStore.GetVariable(variable.Name);
                existingInstance.SetValue( variable.GetTypedValue(false));
                InvokeVariableLifecycleListenersUpdate(existingInstance, this);
                base.InvokeVariableLifecycleListenersDelete(variable, this, new List<IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>>() { VariablePersistenceListener });
            }
            else
            {
                other.variableStore.AddVariable(variable);
            }
        }

        protected internal virtual void MoveConcurrentLocalVariablesTo(ExecutionEntity other)
        {
            IList<Engine.Impl.Core.Variable.ICoreVariableInstance> variables = variableStore.Variables;
            foreach (VariableInstanceEntity variable in variables)
            {
                if (variable.GetIsConcurrentLocal())
                {
                    MoveVariableTo(variable, other);
                }
            }
        }

        // variables ////////////////////////////////////////////////////////////////

        public virtual void AddVariableListener(IVariableInstanceLifecycleListener<VariableInstanceEntity> listener)
        {
            RegisteredVariableListeners.Add(listener);
        }

        public virtual void RemoveVariableListener(IVariableInstanceLifecycleListener<VariableInstanceEntity> listener)
        {
            RegisteredVariableListeners.Remove(listener);
        }
        [JsonIgnore]
        public virtual bool ExecutingScopeLeafActivity
        {
            get
            {
                return IsActive && GetActivity() != null && GetActivity().IsScope && ActivityInstanceId != null && !(GetActivity().ActivityBehavior is ICompositeActivityBehavior);
            }
        }


        [JsonIgnore]
        protected internal virtual bool AutoFireHistoryEvents
        {
            get
            {
                // as long as the process instance is starting (i.e. before activity instance
                // of the selected initial (start event) is created), the variable scope should
                // not automatic fire history events for variable updates.

                // firing the events is triggered by the processInstanceStart context after
                // the initial activity has been initialized. The effect is that the activity instance id of the
                // historic variable instances will be the activity instance id of the start event.

                // if a variable is updated while the process instance is starting then the
                // update history event is lost and the updated value is handled as initial value.

                ActivityImpl currentActivity = (ActivityImpl)Activity;

                return (StartContext == null || !StartContext.DelayFireHistoricVariableEvents) && (currentActivity == null || IsActivityNoStartEvent(currentActivity) || IsStartEventInValidStateOrNotAsync(currentActivity));
            }
        }

        protected internal virtual bool IsActivityNoStartEvent(ActivityImpl currentActivity)
        {
            return !(currentActivity.ActivityBehavior is NoneStartEventActivityBehavior);
        }

        protected internal virtual bool IsStartEventInValidStateOrNotAsync(ActivityImpl currentActivity)
        {
            return ActivityInstanceState != ActivityInstanceStateFields.Default.StateCode || !currentActivity.AsyncBefore;
        }

        public virtual void FireHistoricVariableInstanceCreateEvents()
        {
            // this method is called by the start context and batch-fires create events
            // for all variable instances
            IList<Engine.Impl.Core.Variable.ICoreVariableInstance> variableInstances = variableStore.Variables;
            VariableInstanceHistoryListener historyListener = new VariableInstanceHistoryListener();
            if (variableInstances != null)
            {
                foreach (VariableInstanceEntity variable in variableInstances)
                {
                    historyListener.OnCreate(variable, this);
                }
            }
        }

        /// <summary>
        /// Fetch all the executions inside the same process instance as list and then
        /// reconstruct the complete execution tree.
        /// 
        /// In many cases this is an optimization over fetching the execution tree
        /// lazily. Usually we need all executions anyway and it is preferable to fetch
        /// more data in a single query (maybe even too much data) then to run multiple
        /// queries, each returning a fraction of the data.
        /// 
        /// The most important consideration here is network roundtrip: If the process
        /// engine and database run on separate hosts, network roundtrip has to be
        /// added to each query. Economizing on the number of queries economizes on
        /// network roundtrip. The tradeoff here is network roundtrip vs. throughput:
        /// multiple roundtrips carrying small chucks of data vs. a single roundtrip
        /// carrying more data.
        /// 
        /// </summary>
        protected internal virtual void EnsureExecutionTreeInitialized()
        {
            IList<ExecutionEntity> executions = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionsByProcessInstanceId(ProcessInstanceId);


            ExecutionEntity processInstance = IsProcessInstanceExecution ? this : null;

            if (processInstance == null)
            {
                foreach (ExecutionEntity execution in executions)
                {
                    if (execution.IsProcessInstanceExecution)
                    {
                        processInstance = execution;
                    }
                }
            }

            processInstance.RestoreProcessInstance(executions, null, null, null, null, null, null);
        }

        /// <summary>
        /// Restores a complete process instance tree including referenced entities.
        /// </summary>
        /// <param name="executions">
        ///   the list of all executions that are part of this process instance.
        ///   Cannot be null, must include the process instance execution itself. </param>
        /// <param name="eventSubscriptions">
        ///   the list of all event subscriptions that are linked to executions which is part of this process instance
        ///   If null, event subscriptions are not initialized and lazy loaded on demand </param>
        /// <param name="variables">
        ///   the list of all variables that are linked to executions which are part of this process instance
        ///   If null, variables are not initialized and are lazy loaded on demand </param>
        /// <param name="jobs"> </param>
        /// <param name="tasks"> </param>
        /// <param name="incidents"> </param>
        public virtual void RestoreProcessInstance(ICollection<ExecutionEntity> executions, ICollection<EventSubscriptionEntity> eventSubscriptions, ICollection<VariableInstanceEntity> variables, ICollection<TaskEntity> tasks, ICollection<JobEntity> jobs, ICollection<IncidentEntity> incidents, ICollection<ExternalTaskEntity> externalTasks)
        {

            if (!IsProcessInstanceExecution)
            {
                throw Log.RestoreProcessInstanceException(this);
            }

            // index executions by id
            IDictionary<string, ExecutionEntity> executionsMap = new Dictionary<string, ExecutionEntity>();
            foreach (ExecutionEntity execution in executions)
            {
                executionsMap[execution.Id] = execution;
            }

            IDictionary<string, IList<ICoreVariableInstance>> variablesByScope = new Dictionary<string, IList<ICoreVariableInstance>>();
            if (variables != null)
            {
                foreach (VariableInstanceEntity variable in variables)
                {
                    CollectionUtil.AddToMapOfLists(variablesByScope, variable.VariableScopeId, variable);
                }
            }

            // restore execution tree
            foreach (ExecutionEntity execution in executions)
            {
                if (execution.executions == null)
                {
                    execution.executions = new List<IActivityExecution>();
                }
                if (execution.eventSubscriptions == null && eventSubscriptions != null)
                {
                    execution.eventSubscriptions = new List<EventSubscriptionEntity>();
                }
                if (variables != null)
                {
                    execution.variableStore.VariablesProvider = new VariableCollectionProvider(variablesByScope[execution.Id]);
                }
                ExecutionEntity parentPar = null; ;
                string parentId = execution.ParentId;
                if (parentId != null)
                {
                    parentPar = executionsMap.ContainsKey(parentId) ? executionsMap[parentId] : null;
                }

                if (!execution.IsProcessInstanceExecution)
                {
                    if (parentPar == null)
                        throw Log.ResolveParentOfExecutionFailedException(parentId, execution.Id);
                    execution.processInstance = this;
                    execution.parent = parentPar;
                    if (parentPar.executions == null)
                    {
                        parentPar.executions = new List<IActivityExecution>();
                    }
                    //TODO 可能重复添加execution
                    parentPar.executions.Add(execution);
                }
                else
                {
                    execution.processInstance = execution;
                }
            }

            if (eventSubscriptions != null)
            {
                // add event subscriptions to the right executions in the tree
                foreach (EventSubscriptionEntity eventSubscription in eventSubscriptions)
                {
                    ExecutionEntity executionEntity = executionsMap[eventSubscription.ExecutionId];
                    if (executionEntity != null)
                    {
                        executionEntity.AddEventSubscription(eventSubscription);
                    }
                    else
                    {
                        throw Log.ExecutionNotFoundException(eventSubscription.ExecutionId);
                    }
                }
            }

            if (jobs != null)
            {
                foreach (JobEntity job in jobs)
                {
                    ExecutionEntity execution = executionsMap[job.ExecutionId];
                    job.Execution = execution;
                }
            }

            if (tasks != null)
            {
                foreach (TaskEntity task in tasks)
                {
                    ExecutionEntity execution = executionsMap[task.ExecutionId];
                    task.SetExecution(execution);
                    execution.AddTask(task);

                    if (variables != null)
                    {
                        task.VariableStore.VariablesProvider = new VariableCollectionProvider(variablesByScope[task.Id]);
                    }
                }
            }


            if (incidents != null)
            {
                foreach (IncidentEntity incident in incidents)
                {
                    ExecutionEntity execution = executionsMap[incident.ExecutionId];
                    incident.Execution = execution;
                }
            }

            if (externalTasks != null)
            {
                foreach (ExternalTaskEntity externalTask in externalTasks)
                {
                    ExecutionEntity execution = executionsMap[externalTask.ExecutionId];
                    externalTask.SetExecution(execution);
                    execution.AddExternalTask(externalTask);
                }
            }
        }


        // persistent state /////////////////////////////////////////////////////////

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["processDefinitionId"] = this.ProcessDefinitionId;
            persistentState["businessKey"] = BusinessKey;
            persistentState["activityId"] = this.ActivityId;
            persistentState["ActivityInstanceId"] = this.ActivityInstanceId;
            persistentState["IsActive"] = this.IsActive;
            persistentState["IsConcurrent"] = this.IsConcurrent;
            persistentState["IsScope"] = this.IsScope;
            persistentState["IsEventScope"] = this.IsEventScope;
            persistentState["parentId"] = ParentId;
            persistentState["superExecution"] = this.SuperExecutionId;
            persistentState["superCaseExecutionId"] = this.SuperCaseExecutionId;
            persistentState["caseInstanceId"] = this.CaseInstanceId;
            persistentState["suspensionState"] = this.SuspensionState;
            //persistentState["cachedEntityState"] = CachedEntityState;
            persistentState["sequenceCounter"] = SequenceCounter;
            return persistentState;
        }
        /// <summary>
        /// 先插入数据，后面再关联更新
        /// </summary>
        public virtual void Insert()
        {
            Context.CommandContext.ExecutionManager.InsertExecution(this);
        }

        public override void DeleteCascade2(string deleteReason)
        {
            this.DeleteReason = deleteReason;
            this.IsDeleteRoot = true;
            PerformOperation(new FoxAtomicOperationDeleteCascadeFireActivityEnd());
        }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public override void ForceUpdate()
        {
            //context.Impl.Context.CommandContext.DbEntityManager.ForceUpdate(this);
        }

        // toString /////////////////////////////////////////////////////////////////
        public ICollection<Engine.Impl.Core.Variable.ICoreVariableInstance> ProvideVariables()
        {
            var entities = Context.CommandContext.VariableInstanceManager.FindVariableInstancesByExecutionId(base.Id);
            return entities.Cast<ICoreVariableInstance>().ToList();
        }

        public override string ToString()
        {
            if (IsProcessInstanceExecution)
            {
                return "ProcessInstance[" + ToStringIdentity + "]";
            }
            else
            {
                return (IsConcurrent ? "Concurrent" : "") + (IsScope ? "Scope" : "") + "Execution[" + ToStringIdentity + "]";
            }
        }

        protected internal override string ToStringIdentity
        {
            get
            {
                return Id;
            }
        }

        // event subscription support //////////////////////////////////////////////
        [NotMapped]
        public virtual IList<EventSubscriptionEntity> EventSubscriptionsInternal
        {
            get
            {
                EnsureEventSubscriptionsInitialized();
                return eventSubscriptions;
            }
        }
        [NotMapped]
        public virtual IList<EventSubscriptionEntity> EventSubscriptions
        {
            get
            {
                return EventSubscriptionsInternal;
            }
            set
            {
                this.eventSubscriptions = value;
            }
        }
        [NotMapped]
        public virtual IList<EventSubscriptionEntity> CompensateEventSubscriptions
        {
            get
            {
                IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
                IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>(eventSubscriptions.Count);
                foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
                {
                    if (eventSubscriptionEntity.IsSubscriptionForEventType(EventType.Compensate))
                    {
                        result.Add((EventSubscriptionEntity)eventSubscriptionEntity);
                    }
                }
                return result;
            }
        }

        public virtual IList<EventSubscriptionEntity> GetCompensateEventSubscriptions(string activityId)
        {
            IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
            IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>(eventSubscriptions.Count);
            foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
            {
                if (eventSubscriptionEntity.IsSubscriptionForEventType(EventType.Compensate) && activityId.Equals(eventSubscriptionEntity.ActivityId))
                {
                    result.Add((EventSubscriptionEntity)eventSubscriptionEntity);
                }
            }
            return result;
        }

        protected internal virtual void EnsureEventSubscriptionsInitialized()
        {
            if (eventSubscriptions == null)
            {
                eventSubscriptions = Context.CommandContext.EventSubscriptionManager.FindEventSubscriptionsByExecution(Id);
            }
        }

        public virtual void AddEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
        {
            IList<EventSubscriptionEntity> eventSubscriptionsInternal = EventSubscriptionsInternal;
            if (!eventSubscriptionsInternal.Contains(eventSubscriptionEntity))
            {
                eventSubscriptionsInternal.Add(eventSubscriptionEntity);
            }
        }

        public virtual void RemoveEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
        {
            EventSubscriptionsInternal.Remove(eventSubscriptionEntity);
        }

        // referenced job entities //////////////////////////////////////////////////

        protected internal virtual void EnsureJobsInitialized()
        {
            if (jobs == null)
            {
                jobs = context.Impl.Context.CommandContext.JobManager.FindJobsByExecutionId(Id);
            }
        }
        [NotMapped]
        protected internal virtual IList<JobEntity> JobsInternal
        {
            get
            {
                EnsureJobsInitialized();
                return jobs;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual IList<JobEntity> Jobs
        {
            get
            {
                return new List<JobEntity>(JobsInternal);
            }
        }

        public virtual void AddJob(JobEntity jobEntity)
        {
            IList<JobEntity> jobsInternal = JobsInternal;
            if (!jobsInternal.Contains(jobEntity))
            {
                jobsInternal.Add(jobEntity);
            }
        }

        public virtual void RemoveJob(JobEntity job)
        {
            JobsInternal.Remove(job);
        }

        // referenced incidents entities
        // //////////////////////////////////////////////

        protected internal virtual void EnsureIncidentsInitialized()
        {
            if (incidents == null)
            {
                incidents = context.Impl.Context.CommandContext.IncidentManager.FindIncidentsByExecution(Id);
            }
        }
        [NotMapped]
        protected internal virtual IList<IncidentEntity> IncidentsInternal
        {
            get
            {
                EnsureIncidentsInitialized();
                return incidents;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual IList<IncidentEntity> Incidents
        {
            get
            {
                return new List<IncidentEntity>(IncidentsInternal);
            }
        }

        public virtual void AddIncident(IncidentEntity incident)
        {
            IList<IncidentEntity> incidentsInternal = IncidentsInternal;
            if (!incidentsInternal.Contains(incident))
            {
                incidentsInternal.Add(incident);
            }
        }

        public virtual void RemoveIncident(IncidentEntity incident)
        {
            IncidentsInternal.Remove(incident);
        }

        public virtual IncidentEntity GetIncidentByCauseIncidentId(string causeIncidentId)
        {
            foreach (IncidentEntity incident in Incidents)
            {
                if (incident.CauseIncidentId != null && incident.CauseIncidentId.Equals(causeIncidentId))
                {
                    return incident;
                }
            }
            return null;
        }

        // referenced task entities
        // ///////////////////////////////////////////////////

        protected internal virtual void EnsureTasksInitialized()
        {
            if (tasks == null)
            {
                tasks = Context.CommandContext.TaskManager.FindTasksByExecutionId(Id);
            }
        }

        [NotMapped]
        [JsonIgnore]
        protected internal virtual IList<TaskEntity> TasksInternal
        {
            get
            {
                EnsureTasksInitialized();
                return tasks;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public virtual IList<TaskEntity> Tasks
        {
            get
            {
                return new List<TaskEntity>(TasksInternal);
            }
        }

        public virtual void AddTask(TaskEntity taskEntity)
        {
            IList<TaskEntity> tasksInternal = TasksInternal;
            if (!tasksInternal.Contains(taskEntity))
            {
                tasksInternal.Add(taskEntity);
            }
        }

        public virtual void RemoveTask(TaskEntity task)
        {
            TasksInternal.Remove(task);
        }

        // external tasks

        protected internal virtual void EnsureExternalTasksInitialized()
        {
            if (externalTasks == null)
            {
                externalTasks = Context.CommandContext.ExternalTaskManager.FindExternalTasksByExecutionId(Id);
            }
        }

        protected internal virtual IList<ExternalTaskEntity> ExternalTasksInternal
        {
            get
            {
                EnsureExternalTasksInitialized();
                return externalTasks;
            }
        }

        public virtual void AddExternalTask(ExternalTaskEntity externalTask)
        {
            ExternalTasksInternal.Add(externalTask);
        }

        public virtual void RemoveExternalTask(ExternalTaskEntity externalTask)
        {
            ExternalTasksInternal.Remove(externalTask);
        }

        public override IActivityExecution CreateExecution(bool initializeExecutionStartContext)
        {
            // create the new child execution
            ExecutionEntity createdExecution = CreateNewExecution();

            // initialize sequence counter
            createdExecution.SequenceCounter = SequenceCounter;

            // manage the bidirectional parent-child relation
            createdExecution.SetParent(this);

            // initialize the new execution
            createdExecution.SetProcessDefinition(ProcessDefinition);
            createdExecution.SetProcessInstance(GetProcessInstance());
            createdExecution.SetActivity(GetActivity());
            createdExecution.SuspensionState = SuspensionState;

            // make created execution start in same activity instance
            createdExecution.ActivityInstanceId = ActivityInstanceId;

            // inherit the tenant id from parent execution
            if (TenantId != null)
            {
                createdExecution.TenantId = TenantId;
            }

            if (initializeExecutionStartContext)
            {
                createdExecution.StartContext = new ExecutionStartContext();
            }
            else if (StartContext != null)
            {
                createdExecution.StartContext = StartContext;
            }

            createdExecution.SkipCustomListeners = this.SkipCustomListeners;
            createdExecution.SkipIoMapping = this.SkipIoMapping;

            Log.CreateChildExecution(createdExecution, this);

            return createdExecution;
        }

        public ExecutionEntity GetProcessInstance()
        {
            EnsureProcessInstanceInitialized();
            return processInstance;
        }


        public virtual IList<ExternalTaskEntity> ExternalTasks
        {
            get
            {
                return new List<ExternalTaskEntity>(ExternalTasksInternal);
            }
        }




        // variables /////////////////////////////////////////////////////////

        //protected internal override VariableStore<ICoreVariableInstance> VariableStore
        //{
        //    get
        //    {
        //        //类型转换异常
        //        return variableStore;
        //    }
        //}
        [NotMapped]
        [JsonIgnore]
        protected internal override VariableStore VariableStore
        {
            get
            {
                return variableStore;
            }
        }
        protected VariableStore GetVariableStroe()
        {
            return variableStore;
        }
        [NotMapped]
        [JsonIgnore]
        protected internal override IVariableInstanceFactory<Engine.Impl.Core.Variable.ICoreVariableInstance> VariableInstanceFactory
        {
            get
            {
                return VariableInstanceEntityFactory.Instance;
            }
        }


        protected override IList<IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>> VariableInstanceLifecycleListeners
        {
            get
            {

                IList<IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>> listeners = new List<IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>>();

                listeners.Add(VariablePersistenceListener);
                listeners.Add(new VariableInstanceConcurrentLocalInitializer(this));
                listeners.Add(VariableInstanceSequenceCounterListener.Instance);

                if (AutoFireHistoryEvents)
                {
                    listeners.Add(VariableInstanceHistoryListener.Instance);
                }

                listeners.Add(new VariableListenerInvocationListener(this));

                foreach (IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance> variableInstanceLifecycleListener in RegisteredVariableListeners)
                {
                    listeners.Add(variableInstanceLifecycleListener);
                }

                return listeners;
            }
        }

        public virtual IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance> VariablePersistenceListener
        {
            get
            {
                return VariableInstanceEntityPersistenceListener.Instance as IVariableInstanceLifecycleListener<Engine.Impl.Core.Variable.ICoreVariableInstance>;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual ICollection<Engine.Impl.Core.Variable.ICoreVariableInstance> VariablesInternal
        {
            get
            {
                return variableStore.Variables;
            }
        }

        public virtual void RemoveVariableInternal(VariableInstanceEntity variable)
        {
            if (variableStore.ContainsValue(variable))
            {
                variableStore.RemoveVariable(variable.Name);
            }
        }

        public virtual void AddVariableInternal(VariableInstanceEntity variable)
        {
            if (variableStore.ContainsKey(variable.Name))
            {
                VariableInstanceEntity existingVariable = (VariableInstanceEntity)variableStore.GetVariable(variable.Name);
                existingVariable.SetValue(variable.TypedValue);
                variable.Delete();
            }
            else
            {
                variableStore.AddVariable(variable);
            }
        }

        public virtual void HandleConditionalEventOnVariableChange(VariableEvent variableEvent)
        {
            IList<EventSubscriptionEntity> subScriptions = EventSubscriptions;
            foreach (EventSubscriptionEntity subscription in subScriptions)
            {
                if (EventType.Conditonal.Name.Equals(subscription.EventType))
                {
                    subscription.ProcessEventSync(variableEvent);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableEvent"></param>
        public override void DispatchEvent(VariableEvent variableEvent)
        {
            IList<ExecutionEntity> execs = new List<ExecutionEntity>();
            (new ExecutionTopDownWalker(this)).AddPreVisitor(new TreeVisitorAnonymousInnerClassHelper(this, execs)).WalkUntil();
            foreach (ExecutionEntity execution in execs)
            {
                execution.HandleConditionalEventOnVariableChange(variableEvent);
            }
        }


        private class TreeVisitorAnonymousInnerClassHelper : ITreeVisitor<ExecutionEntity>
        {
            private readonly ExecutionEntity _outerInstance;

            private IList<ExecutionEntity> _execs;

            public TreeVisitorAnonymousInnerClassHelper(ExecutionEntity outerInstance, IList<ExecutionEntity> execs)
            {
                this._outerInstance = outerInstance;
                this._execs = execs;
            }

            public void Visit(ExecutionEntity obj)
            {
                if (obj.EventSubscriptions.Count > 0 && (obj.IsInState(ActivityInstanceStateFields.Default) || (!obj.Activity.IsScope))) // state is default or tree is compacted
                {
                    _execs.Add(obj);
                }
            }
        }



        // getters and setters //////////////////////////////////////////////////////




        public virtual int CachedEntityStateRaw
        {
            get
            {
                return cachedEntityState;
            }
        }

        public override string ProcessInstanceId { get; set; }
        public override string ParentId { get; set; }
        public void SetParentId(string parentId)
        {
            this.ParentId = parentId;
        }

        public override void SetParentExecution(IActivityExecution parent)
        {
            this.parent = parent as ExecutionEntity;

            if (parent != null)
            {
                this.ParentId = parent.Id;
            }
            else
            {
                this.ParentId = null;
            }
        }

        public ISet<string> ReferencedEntityIds
        {
            get
            {
                ISet<string> referenceIds = new HashSet<string>();

                if (SuperExecutionId != null)
                {
                    referenceIds.Add(SuperExecutionId);
                }
                if (ParentId != null)
                {
                    referenceIds.Add(ParentId);
                }

                return referenceIds;
            }
        }
        public override bool IsSuspended
        {
            get
            {
                return SuspensionState == SuspensionStateFields.Suspended.StateCode;
            }
        }
        [JsonIgnore]
        public bool IsInTransaction
        {
            get
            {
                return FlowScope.IsTransaction;
            }
        }
        [JsonIgnore]
        public override ProcessInstanceStartContext ProcessInstanceStartContext
        {
            get
            {
                if (IsProcessInstanceExecution)
                {
                    if (StartContext == null)
                    {
                        StartContext = new ProcessInstanceStartContext((ActivityImpl)Activity);

                    }
                }
                return base.ProcessInstanceStartContext;
            }
        }

        public override string CurrentActivityId
        {
            get
            {
                return ActivityId;
            }
        }

        public override string CurrentActivityName
        {
            get
            {
                return ActivityName;
            }
        }

        [JsonIgnore]
        public override IFlowElement BpmnModelElementInstance
        {
            get
            {
                IBpmnModelInstance bpmnModelInstance = BpmnModelInstance;
                if (bpmnModelInstance != null)
                {

                    IModelElementInstance modelElementInstance = null;
                    if (ExecutionListenerFields.EventNameTake.Equals(EventName))
                    {
                        modelElementInstance = bpmnModelInstance.GetModelElementById(Transition.Id);
                    }
                    else
                    {
                        modelElementInstance = bpmnModelInstance.GetModelElementById(ActivityId);
                    }

                    try
                    {
                        return (IFlowElement)modelElementInstance;

                    }
                    catch (System.InvalidCastException e)
                    {
                        IModelElementType elementType = modelElementInstance.ElementType;
                        throw Log.CastModelInstanceException(modelElementInstance, "FlowElement", elementType.TypeName, elementType.TypeNamespace, e);
                    }

                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public override IBpmnModelInstance BpmnModelInstance
        {
            get
            {
                if (ProcessDefinitionId != null)
                {
                    return Context.ProcessEngineConfiguration.DeploymentCache.FindBpmnModelInstanceForProcessDefinition(ProcessDefinitionId);

                }
                else
                {
                    return null;

                }
            }
        }

        [JsonIgnore]
        public override IProcessEngineServices ProcessEngineServices
        {
            get
            {
                return Context.ProcessEngineConfiguration.ProcessEngine;
            }
        }

        public override IActivityExecution ReplacedBy { get; set; }


        public override IncidentEntity FindLastIncident()
        {
            if (incidents.Count > 0)
            {
                return incidents[incidents.Count - 1];
            }
            return null;
        }
        //public void SetVariable(string key,object value)
        //{
        //    this.Variables.Add(key, value);
        //}
    }

}