using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Runtime;
using Newtonsoft.Json;
using System.Threading;
using ESS.FW.Bpm.Engine.Exception;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class PvmExecutionImpl : CoreExecution, IActivityExecution
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        // tree compaction & expansion ///////////////////////////////////////////

        /// <summary>
        ///     <para>Returns an execution that has replaced this execution for executing activities in their shared scope.</para>
        ///     <para>Invariant: this execution and getReplacedBy() execute in the same scope.</para>
        /// </summary>
        public abstract IActivityExecution ReplacedBy { get; set; }

        public virtual bool ReplacedByParent => (ReplacedBy != null) && (ReplacedBy == Parent);

        public virtual IList<IActivityExecution> Executions { get; } = new List<IActivityExecution>();
        [JsonIgnore]
        public virtual IList<IActivityExecution> AllChildExecutions
        {
            get
            {
                IList<IActivityExecution> childExecutions = new List<IActivityExecution>();
                foreach (var childExecution in Executions)
                {
                    childExecutions.Add(childExecution);
                    ((List<IActivityExecution>) childExecutions).AddRange(childExecution.AllChildExecutions);
                }
                return childExecutions;
            }
        }

        // executions ///////////////////////////////////////////////////////////////

        public abstract IList<IActivityExecution> ExecutionsAsCopy { get; }

        [JsonIgnore]
        public virtual IList<IActivityExecution> NonEventScopeExecutions
        {
            get
            {
                var children = Executions;
                IList<IActivityExecution> result = new List<IActivityExecution>();

                foreach (var child in children)
                    if (!child.IsEventScope)
                        result.Add(child);

                return result;
            }
        }

        [JsonIgnore]
        public virtual IList<IActivityExecution> EventScopeExecutions
        {
            get
            {
                var children = Executions;
                IList<IActivityExecution> result = new List<IActivityExecution>();

                foreach (var child in children)
                    if (child.IsEventScope)
                        result.Add(child);

                return result;
            }
        }

        // process definition ///////////////////////////////////////////////////////
        [NotMapped]
        public virtual ProcessDefinitionImpl ProcessDefinition { get; set; }


        // process instance /////////////////////////////////////////////////////////

        /// <summary>
        ///     ensures initialization and returns the process instance.
        /// </summary>
        public virtual IDelegateExecution ProcessInstance { get; set; }

        // case instance id /////////////////////////////////////////////////////////
        /// <summary>
        ///     the id of a case associated with this execution
        /// </summary>
        public virtual string CaseInstanceId { get; set; }

        public virtual string ActivityId
        {
            get { return Activity?.Id; }
            set { throw new NotImplementedException(); }
        }



        public virtual IActivityExecution Parent { get; set; }
        public abstract void SetParentExecution(IActivityExecution parent);
        /// <summary>
        ///     Sets the execution's parent and updates the old and new parents' set of
        ///     child executions
        /// </summary>
        public void SetParent(IActivityExecution parent)
        {
            var currentParent = Parent;
            //ParentExecution = parent;
            SetParentExecution(parent);
            if (currentParent != null)
            {
                currentParent.Executions.Remove(this);
            }
            if (parent != null)
            {
                parent.Executions.Add(this);
            }
        }

        /// <summary>
        ///     Use #setParent to also update the child execution sets
        /// </summary>
        [NotMapped]
        public abstract IActivityExecution ParentExecution { get; set; }

        // super- and subprocess executions /////////////////////////////////////////

        public virtual IDelegateExecution SuperExecution { get; set; }

        public abstract IActivityExecution SubProcessInstance { get; set; }


        // super case execution /////////////////////////////////////////////////////

        //public abstract CmmnExecution SuperCaseExecution { get; set; }


        // sub case execution ///////////////////////////////////////////////////////

        //public abstract CmmnExecution SubCaseInstance { get; set; }


        // scopes ///////////////////////////////////////////////////////////////////

        protected internal virtual ScopeImpl ScopeActivity
        {
            get
            {
                ScopeImpl scope = null;
                // this if condition is important during process instance startup
                // where the activity of the process instance execution may not be aligned
                // with the execution tree
                if (IsProcessInstanceExecution)
                    scope = ProcessDefinition;
                else
                    scope = (ScopeImpl) Activity;
                return scope;
            }
        }
        [JsonIgnore]
        public virtual IActivityExecution FlowScopeExecution
        {
            get
            {
                if (!IsScope || CompensationBehavior.executesNonScopeCompensationHandler(this))
                    return Parent.FlowScopeExecution;
                return this;
            }
        }

        [JsonIgnore]
        protected internal virtual ScopeImpl FlowScope
        {
            get
            {
                var acti = Activity;

                if (!acti.IsScope || ReferenceEquals(ActivityInstanceId, null) ||
                    (acti.IsScope && !IsScope && acti.ActivityBehavior is ICompositeActivityBehavior))
                    return acti.FlowScope;
                return (ScopeImpl) acti;
            }
        }

        protected internal virtual string ToStringIdentity => Id;

        [JsonIgnore]
        public override AbstractVariableScope ParentVariableScope => (AbstractVariableScope) Parent;


        // sequence counter ///////////////////////////////////////////////////////////
        public virtual long SequenceCounter { get; set; }

        // Getter / Setters ///////////////////////////////////

        [NotMapped]
        public virtual bool ExternallyTerminated { get; set; }

        [NotMapped]
        public virtual string DeleteReason { get; set; }

        [NotMapped]
        public virtual bool IsDeleteRoot { get; set; }

        /// <summary>
        ///     A list of outgoing transitions from the current activity
        ///     that are going to be taken
        /// </summary>
        [NotMapped]
        public virtual IList<IPvmTransition> TransitionsToTake { get; set; } = new List<IPvmTransition>();


        /// <summary>
        ///     transient; used for process instance modification to preserve a scope from getting deleted
        /// </summary>
        [NotMapped]
        public virtual bool PreserveScope { get; set; }

        /// <summary>
        ///     marks the current activity instance
        /// </summary>
        [NotMapped]
        public virtual int ActivityInstanceState { get; set; } = ActivityInstanceStateFields.Default.StateCode;

        public virtual bool IsEventScope { get; set; } = false;

        [NotMapped]
        [JsonIgnore]
        public virtual ExecutionStartContext ExecutionStartContext => StartContext;

        [JsonIgnore]
        public virtual ProcessInstanceStartContext ProcessInstanceStartContext
        {
            get
            {
                if ((StartContext != null) && StartContext is ProcessInstanceStartContext)
                    return (ProcessInstanceStartContext) StartContext;
                return null;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public virtual ExecutionStartContext StartContext { get; set; }

        public virtual bool IsScope { get; set; } = true;

        public virtual bool IsConcurrent { get; set; } = false;

        /// <summary>
        ///     indicates if this execution represents an active path of execution.
        ///     Executions are made inactive in the following situations:
        ///     <ul>
        ///         <li>an execution enters a nested scope</li>
        ///         <li>an execution is split up into multiple concurrent executions, then the parent is made inactive.</li>
        ///         <li>an execution has arrived in a parallel gateway or join and that join has not yet activated/fired.</li>
        ///         <li>an execution is ended.</li>
        ///     </ul>
        /// </summary>
        public virtual bool IsActive { get; set; } = true;

        [NotMapped]
        public virtual bool IsCompleteScope
        {
            get { return ActivityInstanceStateFields.ScopeComplete.StateCode == ActivityInstanceState; }
            set
            {
                if (value && !Canceled)
                    ActivityInstanceState = ActivityInstanceStateFields.ScopeComplete.StateCode;
            }
        }

        /// <summary>
        ///     the activity which is to be started next
        /// </summary>
        [NotMapped]
        public virtual IPvmActivity NextActivity { get; set; }

        public virtual bool IsProcessInstanceExecution => Parent == null;



        /// <summary>
        ///     the Transition that is currently being taken
        /// </summary>
        public IPvmTransition Transition { get; set; }

        //public abstract void start(IDictionary<string, object> variables);
        public abstract IProcessEngineServices ProcessEngineServices { get; }
        public abstract IFlowElement BpmnModelElementInstance { get; }
        public abstract IBpmnModelInstance BpmnModelInstance { get; }
        public abstract string ProcessDefinitionId { get; set; }
        public abstract string ProcessInstanceId { get; set; }
        //public abstract string GetProcessInstanceId();
        // business key /////////////////////////////////////////

        public virtual string ProcessBusinessKey => BusinessKey;

        public virtual string CurrentActivityName
        {
            get
            {
                var acti = Activity;
                return acti?.Name;
            }
        }

        public virtual string CurrentActivityId => ActivityId;
        
        [JsonIgnore]
        public virtual string ParentActivityInstanceId
        {
            get
            {
                if (IsProcessInstanceExecution)
                    return Id;
                return Parent.ActivityInstanceId;
            }
        }

        /// <summary>
        ///     the unique id of the current activity instance
        /// </summary>
        public virtual string ActivityInstanceId { get; set; }


        // parent ///////////////////////////////////////////////////////////////////

        /// <summary>
        ///     ensures initialization and returns the parent
        /// </summary>
        public virtual string ParentId
        {
            get
            {
                var parent = Parent;
                return parent?.Id;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        // variables ////////////////////////////////////////////

        public override string VariableScopeKey => "execution";

        public virtual void SetVariable(string variableName, object value, string activityId)
        {
            var executionForFlowScope = FindExecutionForFlowScope(activityId);
            executionForFlowScope?.SetVariableLocal(variableName, value);
        }

        [JsonIgnore]
        public virtual string CurrentTransitionId => Transition.Id;


        [NotMapped]
        public virtual bool Canceled
        {
            get { return ActivityInstanceStateFields.Canceled.StateCode == ActivityInstanceState; }
            set
            {
                if (value)
                    ActivityInstanceState = ActivityInstanceStateFields.Canceled.StateCode;
            }
        }




        // methods that translate to operations /////////////////////////////////////

        public virtual void Signal(string signalName, object signalData)
        {
            if (Activity == null)
                throw new PvmException("cannot signal execution " + Id + ": it has no current activity");

            var activityBehavior = (ISignallableActivityBehavior) Activity.ActivityBehavior;
            try
            {
                activityBehavior.Signal(this, signalName, signalData);
            }
            catch (RuntimeException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new PvmException(
                    "couldn't process signal '" + signalName + "' on activity '" + Activity.Id + "': " + e.Message, e);
            }
        }

        [NotMapped]
        public virtual bool IsEnded { get; set; } = false;

        public virtual void Start()
        {
            Start(null);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="variables"></param>
        public virtual void Start(IDictionary<string, object> variables)
        {
            StartContext = new ProcessInstanceStartContext((ActivityImpl) Activity);

            Initialize();
            InitializeTimerDeclarations();

            if (variables != null)
                Variables = variables;

            FireHistoricProcessStartEvent();
            PerformOperation(PvmAtomicOperationFields.ProcessStart);
        }

        public virtual void DeleteCascade(string deleteReason)
        {
            DeleteCascade(deleteReason, false);
        }

        public virtual IList<IPvmExecution> FindExecutions(string activityId)
        {
            IList<IPvmExecution> matchingExecutions = new List<IPvmExecution>();
            CollectExecutions(activityId, matchingExecutions);

            return matchingExecutions;
        }

        public virtual IList<string> FindActiveActivityIds()
        {
            IList<string> activeActivityIds = new List<string>();
            CollectActiveActivityIds(activeActivityIds);
            return activeActivityIds;
        }

        public abstract void ForceUpdate();

        // API ////////////////////////////////////////////////

        /// <summary>
        ///     creates a new execution. properties processDefinition, processInstance and activity will be initialized.
        /// </summary>
        public virtual IActivityExecution CreateExecution()
        {
            return CreateExecution(false);
        }

        public abstract IActivityExecution CreateExecution(bool initStartContext);

        // sub process instance

        public virtual IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition)
        {
            return CreateSubProcessInstance(processDefinition, null);
        }

        public virtual IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition,
            string businessKey)
        {
            var processInstance = (PvmExecutionImpl) ProcessInstance;

            string caseInstanceId = null;
            if (processInstance != null)
                caseInstanceId = processInstance.CaseInstanceId;

            return CreateSubProcessInstance(processDefinition, businessKey, caseInstanceId);
        }

        public virtual IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition,
            string businessKey, string caseInstanceId)
        {
            ExecutionEntity subProcessInstance = (ExecutionEntity)NewExecution();

            // manage bidirectional super-subprocess relation
            subProcessInstance.SuperExecution=(this);
            SubProcessInstance = subProcessInstance;

            // Initialize the new execution
            subProcessInstance.ProcessDefinition = (ProcessDefinitionImpl) processDefinition;
            subProcessInstance.ProcessInstance = subProcessInstance;
            subProcessInstance.Activity = processDefinition.Initial;

            if (!ReferenceEquals(businessKey, null))
                subProcessInstance.BusinessKey = businessKey;

            if (!ReferenceEquals(caseInstanceId, null))
                subProcessInstance.CaseInstanceId = caseInstanceId;

            return subProcessInstance;
        }

        public virtual void Destroy()
        {
            Log.Destroying(this);
            IsScope = false;
        }

        /// <summary>
        ///     Interrupts an execution
        /// </summary>
        public virtual void Interrupt(string reason)
        {
            Interrupt(reason, false, false);
        }

        /// <summary>
        ///     Ends an execution. Invokes end listeners for the current activity and notifies the flow scope execution
        ///     of this happening which may result in the flow scope ending.
        /// </summary>
        /// <param name="completeScope"> true if ending the execution contributes to completing the BPMN 2.0 scope </param>
        public virtual void End(bool completeScope)
        {
            IsCompleteScope = completeScope;

            IsActive = false;
            IsEnded = true;

            if (HasReplacedParent())
                Parent.ReplacedBy = null;

            PerformOperation(PvmAtomicOperationFields.ActivityNotifyListenerEnd);
        }

        public virtual void EndCompensation()
        {
            PerformOperation(PvmAtomicOperationFields.FireActivityEnd);
            Remove();

            var parent = Parent;

            if (parent.Activity == null)
                parent.Activity = (ActivityImpl) Activity.FlowScope;

            parent.Signal(CompensationUtil.SignalCompensationDone, null);
        }

        public virtual void Remove()
        {
            var parent = Parent;
            if (parent != null)
            {
                parent.Executions.Remove(this);

                // if the sequence counter is greater than the
                // sequence counter of the parent, then set
                // the greater sequence counter on the parent.
                var parentSequenceCounter = parent.SequenceCounter;
                var mySequenceCounter = SequenceCounter;
                if (mySequenceCounter > parentSequenceCounter)
                    parent.SequenceCounter = mySequenceCounter;

                // propagate skipping configuration upwards, if it was not initially set on
                // the root execution
                parent.SkipCustomListeners |= SkipCustomListeners;
                parent.SkipIoMapping |= SkipIoMapping;
            }

            IsActive = false;
            IsEnded = true;

            if (HasReplacedParent())
                Parent.ReplacedBy = null;

            RemoveEventScopes();
        }

        public virtual bool TryPruneLastConcurrentChild()
        {
            if (NonEventScopeExecutions.Count == 1)
            {
                var lastConcurrent = NonEventScopeExecutions[0];
                if (lastConcurrent.IsConcurrent)
                {
                    if (!lastConcurrent.IsScope)
                    {
                        Activity = lastConcurrent.Activity;
                        Transition = (lastConcurrent.Transition);
                        Replace(lastConcurrent);

                        // Move children of lastConcurrent one level up
                        if (lastConcurrent.HasChildren())
                            foreach (var childExecution in lastConcurrent.ExecutionsAsCopy)
                                childExecution.Parent = (this);

                        // Make sure parent execution is re-activated when the last concurrent
                        // child execution is active
                        if (!IsActive && lastConcurrent.IsActive)
                            IsActive = true;

                        lastConcurrent.Remove();
                    }
                    else
                    {
                        // legacy behavior
                        LegacyBehavior.PruneConcurrentScope(lastConcurrent);
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Execute an activity which is not contained in normal flow (having no incoming sequence flows).
        ///     Cannot be called for activities contained in normal flow.
        ///     <para>
        ///         First, the ActivityStartBehavior is evaluated.
        ///         In case the start behavior is not <seealso cref="ActivityStartBehavior#DEFAULT" />, the corresponding start
        ///         behavior is executed before executing the activity.
        ///     </para>
        ///     <para>
        ///         For a given activity, the execution on which this method must be called depends on the type of the start
        ///         behavior:
        ///         <ul>
        ///             <li>CONCURRENT_IN_FLOW_SCOPE: scope execution for <seealso cref="IPvmActivity#getFlowScope()" /></li>
        ///             <li>INTERRUPT_EVENT_SCOPE: scope execution for <seealso cref="IPvmActivity#getEventScope()" /></li>
        ///             <li>CANCEL_EVENT_SCOPE: scope execution for <seealso cref="IPvmActivity#getEventScope()" /></li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="activity"> activity to start </param>
        public virtual void ExecuteActivity(IPvmActivity activity)
        {
            if (activity.IncomingTransitions.Count > 0)
                throw new ProcessEngineException(
                    "Activity is contained in normal flow and cannot be executed using executeActivity().");

            var activityStartBehavior = activity.ActivityStartBehavior;
            if (!IsScope && (ActivityStartBehavior.Default != activityStartBehavior))
                throw new ProcessEngineException("Activity '" + activity + "' with start behavior '" +
                                                 activityStartBehavior + "'" +
                                                 "cannot be executed by non-scope execution.");

            var activityImpl = activity;
            switch (activityStartBehavior)
            {
                case ActivityStartBehavior.ConcurrentInFlowScope:
                    NextActivity = activityImpl;
                    PerformOperation(PvmAtomicOperationFields.ActivityStartConcurrent);
                    break;

                case ActivityStartBehavior.CancelEventScope:
                    NextActivity = activityImpl;
                    PerformOperation(PvmAtomicOperationFields.ActivityStartCancelScope);
                    break;

                case ActivityStartBehavior.InterruptEventScope:
                    NextActivity = activityImpl;
                    PerformOperation(PvmAtomicOperationFields.ActivityStartInterruptScope);
                    break;

                default:
                    Activity = (ActivityImpl) activityImpl;
                    ActivityInstanceId = null;
                    PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
                    break;
            }
        }


        public virtual IList<IActivityExecution> FindInactiveConcurrentExecutions(IPvmActivity activity)
        {
            List<IActivityExecution> inactiveConcurrentExecutionsInActivity = new List<IActivityExecution>();
            if (IsConcurrent)
                return Parent.FindInactiveChildExecutions(activity);
            if (!IsActive)
                inactiveConcurrentExecutionsInActivity.Add(this);

            return inactiveConcurrentExecutionsInActivity;
        }

        public virtual IList<IActivityExecution> FindInactiveChildExecutions(IPvmActivity activity)
        {
            IList<IActivityExecution> inactiveConcurrentExecutionsInActivity = new List<IActivityExecution>();
            var concurrentExecutions = AllChildExecutions;
            foreach (var concurrentExecution in concurrentExecutions)
                if ((concurrentExecution.Activity == activity) && !concurrentExecution.IsActive)
                    inactiveConcurrentExecutionsInActivity.Add(concurrentExecution);

            return inactiveConcurrentExecutionsInActivity;
        }

        public virtual void LeaveActivityViaTransition(IPvmTransition outgoingTransition)
        {
            LeaveActivityViaTransitions(new List<IPvmTransition> {outgoingTransition}, new List<IActivityExecution>());
        }

        public virtual void InActivate()
        {
            IsActive = false;
        }

        public virtual void EnterActivityInstance()
        {
            ActivityInstanceId = GenerateActivityInstanceId(Activity.Id);

            Log.DebugEnterActivityInstance(this, ParentActivityInstanceId);

            // <LEGACY>: in general, io mappings may only exist when the activity is scope
            // however, for multi instance activities, the inner activity does not become a scope
            // due to the presence of an io mapping. In that case, it is ok to execute the io mapping
            // anyway because the multi-instance body already ensures variable isolation
            ExecuteIoMapping();

            if (Activity.IsScope)
                InitializeTimerDeclarations();
        }

        public virtual void LeaveActivityInstance()
        {
            if (!ReferenceEquals(ActivityInstanceId, null))
                Log.DebugLeavesActivityInstance(this, ActivityInstanceId);
            ActivityInstanceId = ParentActivityInstanceId;

            ActivityInstanceState = ActivityInstanceStateFields.Default.StateCode;
        }

        public virtual bool HasChildren()
        {
            return Executions.Count > 0;
        }

        public virtual IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping()
        {
            ScopeImpl currentActivity = (ScopeImpl) Activity;
            EnsureUtil.EnsureNotNull("activity of current execution", currentActivity);

            var flowScope = FlowScope;
            var flowScopeExecution = FlowScopeExecution;

            return flowScopeExecution.CreateActivityExecutionMapping(flowScope);
        }


        protected internal abstract PvmExecutionImpl NewExecution();

        // sub case instance

        //public override abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition);

        //public override abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey);

        public abstract void Initialize();

        public abstract void InitializeTimerDeclarations();

        public virtual void ExecuteIoMapping()
        {
            // execute Input Mappings (if they exist).
            var currentScope = ScopeActivity;
            if (currentScope != currentScope.ProcessDefinition)
            {
                var currentActivity = (ActivityImpl) currentScope;

                if ((currentActivity != null) && (currentActivity.ioMapping != null) && !SkipIoMapping)
                    currentActivity.ioMapping.ExecuteInputParameters(this);
            }
        }

        /// <summary>
        ///     perform starting behavior but don't execute the initial activity
        /// </summary>
        /// <param name="variables"> the variables which are used for the start </param>
        public virtual void StartWithoutExecuting(IDictionary<string, object> variables)
        {
            Initialize();
            InitializeTimerDeclarations();
            FireHistoricProcessStartEvent();
            PerformOperation(PvmAtomicOperationFields.FireProcessStart);

            Activity = (null);
            ActivityInstanceId = Id;

            // set variables
            Variables = variables;
        }

        public abstract void FireHistoricProcessStartEvent();

        protected internal virtual void RemoveEventScopes()
        {
            IList<IActivityExecution> childExecutions = new List<IActivityExecution>(EventScopeExecutions);
            foreach (var childExecution in childExecutions)
            {
                Log.RemovingEventScope(childExecution);
                childExecution.Destroy();
                childExecution.Remove();
            }
        }

        public virtual void ClearScope(string reason, bool skipCustomListeners, bool skipIoMappings)
        {
            this.SkipCustomListeners = skipCustomListeners;
            SkipIoMapping = skipIoMappings;

            SubProcessInstance?.DeleteCascade(reason, skipCustomListeners, skipIoMappings);

            // remove all child executions and sub process instances:
            IList<IActivityExecution> executions = new List<IActivityExecution>(NonEventScopeExecutions);
            foreach (var childExecution in executions)
            {
                childExecution.SubProcessInstance?.DeleteCascade(reason, skipCustomListeners, skipIoMappings);
                childExecution.DeleteCascade(reason, skipCustomListeners, skipIoMappings);
            }

            // fire activity end on active activity
            if (IsActive && (Activity != null))
            {
                // set activity instance state to cancel
                if (ActivityInstanceState != ActivityInstanceStateFields.Ending.StateCode)
                {
                    Canceled = true;
                    PerformOperation(PvmAtomicOperationFields.FireActivityEnd);
                }
                // set activity instance state back to 'default'
                // -> execution will be reused for executing more activities and we want the state to
                // be default initially.
                ActivityInstanceState = ActivityInstanceStateFields.Default.StateCode;
            }
        }

        public virtual void Interrupt(string reason, bool skipCustomListener, bool skipIoMappings)
        {
            Log.InterruptingExecution(reason, skipCustomListener);

            ClearScope(reason, skipCustomListener, skipIoMappings);
        }

        /// <summary>
        ///     <para>Precondition: execution is already ended but this has not been propagated yet.</para>
        ///     <para>
        ///     </para>
        ///     <para>
        ///         Propagates the ending of this execution to the flowscope execution; currently only supports
        ///         the process instance execution
        ///     </para>
        /// </summary>
        public virtual void PropagateEnd()
        {
            if (!IsEnded)
                throw new ProcessEngineException(ToString() + " must have ended before ending can be propagated");

            if (IsProcessInstanceExecution)
                PerformOperation(PvmAtomicOperationFields.ProcessEnd);
        }

        public virtual IActivityExecution CreateConcurrentExecution()
        {
            if (!IsScope)
                throw new ProcessEngineException("Cannot create concurrent execution for " + this);

            // The following covers the three cases in which a concurrent execution may be created
            // (this execution is the root in each scenario).
            //
            // Note: this should only consider non-event-scope executions. Event-scope executions
            // are not relevant for the tree structure and should remain under their original parent.
            //
            //
            // (1) A compacted tree:
            //
            // Before:               After:
            //       -------               -------
            //       |  e1  |              |  e1 |
            //       -------               -------
            //                             /     \
            //                         -------  -------
            //                         |  e2 |  |  e3 |
            //                         -------  -------
            //
            // e2 replaces e1; e3 is the new root for the activity stack to instantiate
            //
            //
            // (2) A single child that is a scope execution
            // Before:               After:
            //       -------               -------
            //       |  e1 |               |  e1 |
            //       -------               -------
            //          |                  /     \
            //       -------           -------  -------
            //       |  e2 |           |  e3 |  |  e4 |
            //       -------           -------  -------
            //                            |
            //                         -------
            //                         |  e2 |
            //                         -------
            //
            //
            // e3 is created and is concurrent;
            // e4 is the new root for the activity stack to instantiate
            //
            // (3) Existing concurrent execution(s)
            // Before:               After:
            //       -------                    ---------
            //       |  e1 |                    |   e1  |
            //       -------                    ---------
            //       /     \                   /    |    \
            //  -------    -------      -------  -------  -------
            //  |  e2 | .. |  eX |      |  e2 |..|  eX |  | eX+1|
            //  -------    -------      -------  -------  -------
            //
            // eX+1 is concurrent and the new root for the activity stack to instantiate
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: List<? extends PvmExecutionImpl> children = this.getNonEventScopeExecutions();
            var children = NonEventScopeExecutions;

            // whenever we change the set of child executions we have to force an update
            // on the scope executions to avoid concurrent modifications (e.g. tree compaction)
            // that go unnoticed
            //TODO 强制更新
            ForceUpdate();

            if (children.Count == 0)
            {
                // (1)
                var replacingExecution = (PvmExecutionImpl) CreateExecution();
                replacingExecution.IsConcurrent = true;
                replacingExecution.IsScope = false;
                replacingExecution.Replace(this);
                InActivate();
                Activity = (null);
            }
            else if (children.Count == 1)
            {
                // (2)
                var child = children[0];

                var concurrentReplacingExecution = (PvmExecutionImpl) CreateExecution();
                concurrentReplacingExecution.IsConcurrent = true;
                concurrentReplacingExecution.IsScope = false;
                concurrentReplacingExecution.IsActive = false;
                concurrentReplacingExecution.OnConcurrentExpand(this);
                child.Parent = (concurrentReplacingExecution);
                LeaveActivityInstance();
                Activity = (null);
            }

            // (1), (2), and (3)
            var concurrentExecution = (PvmExecutionImpl) CreateExecution();
            concurrentExecution.IsConcurrent = true;
            concurrentExecution.IsScope = false;

            return concurrentExecution;
        }

        public virtual void DeleteCascade(string deleteReason, bool skipCustomListener)
        {
            DeleteCascade(deleteReason, skipCustomListener, false);
        }

        public virtual void DeleteCascade(string deleteReason, bool skipCustomListener, bool skipIoMappings)
        {
            DeleteCascade(deleteReason, skipCustomListener, skipIoMappings, false);
        }

        public virtual void DeleteCascade(string deleteReason, bool skipCustomListener, bool skipIoMappings,
            bool externallyTerminated)
        {
            this.DeleteReason = deleteReason;
            IsDeleteRoot = true;
            IsEnded = true;
            this.SkipCustomListeners = skipCustomListener;
            SkipIoMapping = skipIoMappings;
            this.ExternallyTerminated = externallyTerminated;
            PerformOperation(PvmAtomicOperationFields.DeleteCascade);
        }

        public virtual void DeleteCascade2(string deleteReason)
        {
            this.DeleteReason = deleteReason;
            IsDeleteRoot = true;
            PerformOperation(new FoxAtomicOperationDeleteCascadeFireActivityEnd());
        }

        public virtual void ExecuteEventHandlerActivity(ActivityImpl eventHandlerActivity)
        {
            // the target scope
            var flowScope = eventHandlerActivity.FlowScope;

            // the event scope (the current activity)
            var eventScope = (ScopeImpl) eventHandlerActivity.EventScope;

            if ((eventHandlerActivity.ActivityStartBehavior == ActivityStartBehavior.ConcurrentInFlowScope) &&
                (flowScope != eventScope))
                FindExecutionForScope(eventScope, flowScope)
                    .ExecuteActivity(eventHandlerActivity);
            else
                ExecuteActivity(eventHandlerActivity);
        }

        /// <summary>
        ///     Instead of <seealso cref="#getReplacedBy()" />, which returns the execution that this execution was directly
        ///     replaced with,
        ///     this resolves the chain of replacements (i.e. in the case the replacedBy execution itself was replaced again)
        /// </summary>
        public virtual IActivityExecution ResolveReplacedBy()
        {
            // follow the links of execution replacement;
            // note: this can be at most two hops:
            // case 1:
            //   this execution is a scope execution
            //     => tree may have expanded meanwhile
            //     => scope execution references replacing execution directly (one hop)
            //
            // case 2:
            //   this execution is a concurrent execution
            //     => tree may have compacted meanwhile
            //     => concurrent execution references scope execution directly (one hop)
            //
            // case 3:
            //   this execution is a concurrent execution
            //     => tree may have compacted/expanded/compacted/../expanded any number of times
            //     => the concurrent execution has been removed and therefore references the scope execution (first hop)
            //     => the scope execution may have been replaced itself again with another concurrent execution (second hop)
            //   note that the scope execution may have a long "history" of replacements, but only the last replacement is relevant here
            var replacingExecution = ReplacedBy;

            if (replacingExecution != null)
            {
                var secondHopReplacingExecution = replacingExecution.ReplacedBy;
                if (secondHopReplacingExecution != null)
                    replacingExecution = secondHopReplacingExecution;
            }

            return replacingExecution;
        }

        public virtual bool HasReplacedParent()
        {
            return (Parent != null) && (Parent.ReplacedBy == this);
        }

        /// <summary>
        ///     <para>
        ///         Replace an execution by this execution. The replaced execution has a pointer (
        ///         <seealso cref="#getReplacedBy()" />) to this execution.
        ///         This pointer is maintained until the replaced execution is removed or this execution is removed/ended.
        ///     </para>
        ///     <para>
        ///     </para>
        ///     <para>This is used for two cases: Execution tree expansion and execution tree compaction</para>
        ///     <ul>
        ///         <li>
        ///             <b>expansion</b>: Before:
        ///             <pre>
        ///                 -------
        ///                 |  e1 |  scope
        ///                 -------
        ///             </pre>
        ///             After:
        ///             <pre>
        ///                 -------
        ///                 |  e1 |  scope
        ///                 -------
        ///                 |
        ///                 -------
        ///                 |  e2 |  cc (no scope)
        ///                 -------
        ///             </pre>
        ///             e2 replaces e1: it should receive all entities associated with the activity currently executed
        ///             by e1; these are tasks, (local) variables, jobs (specific for the activity, not the scope)
        ///         </li>
        ///         <li>
        ///             <b>compaction</b>: Before:
        ///             <pre>
        ///                 -------
        ///                 |  e1 |  scope
        ///                 -------
        ///                 |
        ///                 -------
        ///                 |  e2 |  cc (no scope)
        ///                 -------
        ///             </pre>
        ///             After:
        ///             <pre>
        ///                 -------
        ///                 |  e1 |  scope
        ///                 -------
        ///             </pre>
        ///             e1 replaces e2: it should receive all entities associated with the activity currently executed
        ///             by e2; these are tasks, (all) variables, all jobs
        ///         </li>
        ///     </ul>
        /// </summary>
        /// <seealso cref= #createConcurrentExecution
        /// (
        /// )
        /// </seealso>
        /// <seealso cref= #tryPruneLastConcurrentChild
        /// (
        /// )
        /// </seealso>
        public virtual void Replace(IActivityExecution execution)
        {
            // activity instance id handling
            ActivityInstanceId = execution.ActivityInstanceId;
            IsActive = execution.IsActive;

            ReplacedBy = null;
            execution.ReplacedBy = this;

            TransitionsToTake = execution.TransitionsToTake;

            execution.LeaveActivityInstance();
        }

        /// <summary>
        ///     Callback on tree expansion when this execution is used as the concurrent execution
        ///     where the argument's children become a subordinate to. Note that this case is not the inverse
        ///     of replace because replace has the semantics that the replacing execution can be used to continue
        ///     execution of this execution's activity instance.
        /// </summary>
        public virtual void OnConcurrentExpand(PvmExecutionImpl scopeExecution)
        {
            // by default, do nothing
        }

        public virtual void Take()
        {
            if (Transition == null)
                throw new PvmException(ToString() + ": no Transition to take specified");
            if (Transition == null)
                throw new PvmException("Transition is null");
            var transitionImpl = Transition;
            Activity = (transitionImpl.Source);
            // while executing the Transition, the activityInstance is 'null'
            // (we are not executing an activity)
            ActivityInstanceId = null;
            IsActive = true;
            PerformOperation(PvmAtomicOperationFields.TransitionNotifyListenerTake);
        }

        /// <summary>
        ///     Instantiates the given activity stack under this execution.
        ///     Sets the variables for the execution responsible to execute the most deeply nested
        ///     activity.
        /// </summary>
        /// <param name="activityStack"> The most deeply nested activity is the last element in the list </param>
        /// <param name="targetActivity"></param>
        /// <param name="targetTransition"></param>
        /// <param name="variables"></param>
        /// <param name="localVariables"></param>
        /// <param name="skipCustomListener"></param>
        /// <param name="skipIoMappings"></param>
        public virtual void ExecuteActivitiesConcurrent(IList<IPvmActivity> activityStack, IPvmActivity targetActivity,
            IPvmTransition targetTransition, IDictionary<string, object> variables,
            IDictionary<string, object> localVariables, bool skipCustomListener, bool skipIoMappings)
        {
            ScopeImpl flowScope = null;
            if (activityStack.Count > 0)
                flowScope = activityStack[0].FlowScope;
            else if (targetActivity != null)
                flowScope = targetActivity.FlowScope;
            else if (targetTransition != null)
                flowScope = targetTransition.Source.FlowScope;

            IActivityExecution propagatingExecution = null;
            if (flowScope.ActivityBehavior is IModificationObserverBehavior)
            {
                var flowScopeBehavior = (IModificationObserverBehavior) flowScope.ActivityBehavior;
                propagatingExecution = (PvmExecutionImpl) flowScopeBehavior.CreateInnerInstance(this);
            }
            else
            {
                propagatingExecution = CreateConcurrentExecution();
            }

            propagatingExecution.ExecuteActivities(activityStack, targetActivity, targetTransition, variables,
                localVariables, SkipCustomListeners, skipIoMappings);
        }

        /// <summary>
        ///     Instantiates the given set of activities and returns the execution for the bottom-most activity
        /// </summary>
        public virtual IDictionary<IPvmActivity, PvmExecutionImpl> InstantiateScopes(IList<IPvmActivity> activityStack,
            bool skipCustomListener, bool skipIoMappings)
        {
            if (activityStack.Count == 0)
                return null;

            this.SkipCustomListeners = skipCustomListener;
            SkipIoMapping = skipIoMappings;

            var executionStartContext = new ExecutionStartContext(false);

            var instantiationStack = new InstantiationStack(new List<IPvmActivity>(activityStack));
            executionStartContext.InstantiationStack = instantiationStack;
            StartContext = executionStartContext;

            PerformOperation(PvmAtomicOperationFields.ActivityInitStackAndReturn);

            IDictionary<IPvmActivity, PvmExecutionImpl> createdExecutions =
                new Dictionary<IPvmActivity, PvmExecutionImpl>();

            var currentExecution = this;
            foreach (var instantiatedActivity in activityStack)
            {
                // there must exactly one child execution
                currentExecution = (PvmExecutionImpl) currentExecution.NonEventScopeExecutions[0];
                if (currentExecution.IsConcurrent)
                    currentExecution = (PvmExecutionImpl) currentExecution.NonEventScopeExecutions[0];

                createdExecutions[instantiatedActivity] = currentExecution;
            }

            return createdExecutions;
        }

        /// <summary>
        ///     Instantiates the given activity stack. Uses this execution to execute the
        ///     highest activity in the stack.
        ///     Sets the variables for the execution responsible to execute the most deeply nested
        ///     activity.
        /// </summary>
        /// <param name="activityStack"> The most deeply nested activity is the last element in the list </param>
        public virtual void ExecuteActivities(IList<IPvmActivity> activityStack, IPvmActivity targetActivity,
            IPvmTransition targetTransition, IDictionary<string, object> variables,
            IDictionary<string, object> localVariables, bool skipCustomListeners, bool skipIoMappings)
        {
            this.SkipCustomListeners = skipCustomListeners;
            SkipIoMapping = skipIoMappings;
            ActivityInstanceId = null;
            IsEnded = false;

            if (activityStack.Count > 0)
            {
                var executionStartContext = new ExecutionStartContext(false);

                var instantiationStack = new InstantiationStack(activityStack, targetActivity, targetTransition);
                executionStartContext.InstantiationStack = instantiationStack;
                executionStartContext.Variables = variables;
                executionStartContext.VariablesLocal = localVariables;
                StartContext = executionStartContext;

                PerformOperation(PvmAtomicOperationFields.ActivityInitStack);
            }
            else if (targetActivity != null)
            {
                Variables = variables;
                VariablesLocal = localVariables;
                Activity = (ActivityImpl) (targetActivity);
                //TODO 源码调用了ExecutionEntity方法
                PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
            }
            else if (targetTransition != null)
            {
                Variables = variables;
                VariablesLocal = localVariables;
                Activity = (ActivityImpl) (targetTransition.Source);
                Transition = (targetTransition);
                PerformOperation(PvmAtomicOperationFields.TransitionStartNotifyListenerTake);
            }
        }

        public virtual void LeaveActivityViaTransitions(IList<IPvmTransition> transitions,
            IList<IActivityExecution> _recyclableExecutions)
        {
            IList<IActivityExecution> recyclableExecutions = new List<IActivityExecution>();
            if (_recyclableExecutions != null)
                recyclableExecutions = _recyclableExecutions;

            // if recyclable executions size is greater
            // than 1, then the executions are joined and
            // the activity is left with 'this' execution,
            // if it is not not the last concurrent execution.
            // therefore it is necessary to remove the local
            // variables (event if it is the last concurrent
            // execution).
             if (recyclableExecutions.Count > 1)
                RemoveVariablesLocalInternal();

            // mark all recyclable executions as ended
            // if the list of recyclable executions also
            // contains 'this' execution, then 'this' execution
            // is also marked as ended. (if 'this' execution is
            // pruned, then the local variables are not copied
            // to the parent execution)
            // this is a workaround to not delete all recyclable
            // executions and create a new execution which leaves
            // the activity.
            foreach (var execution in recyclableExecutions)
                execution.IsEnded = true;

            // remove 'this' from recyclable executions to
            // leave the activity with 'this' execution
            // (when 'this' execution is the last concurrent
            // execution, then 'this' execution will be pruned,
            // and the activity is left with the scope
            // execution)
            recyclableExecutions.Remove(this);
            foreach (var execution in recyclableExecutions)
                execution.End(transitions.Count == 0);

            var propagatingExecution = this;
            if (ReplacedBy != null)
                propagatingExecution = (PvmExecutionImpl) ReplacedBy;

            propagatingExecution.IsActive = true;
            propagatingExecution.IsEnded = false;

            if (transitions.Count == 0)
            {
                propagatingExecution.End(!propagatingExecution.IsConcurrent);
            }

            else
            {
                propagatingExecution.TransitionsToTake = transitions;
                propagatingExecution.PerformOperation(PvmAtomicOperationFields.TransitionNotifyListenerEnd);
            }
        }

        protected internal abstract void RemoveVariablesLocalInternal();

        public virtual bool IsActiveMethod(string activityId)
        {
            return FindExecution(activityId) != null;
        }

        public virtual IPvmExecution FindExecution(string activityId)
        {
            if ((Activity != null) && Activity.Id.Equals(activityId))
                return this;
            foreach (var nestedExecution in Executions)
            {
                var result = nestedExecution.FindExecution(activityId);
                if (result != null)
                    return result;
            }
            return null;
        }

        public virtual void CollectExecutions(string activityId, IList<IPvmExecution> executions)
        {
            if ((Activity != null) && Activity.Id.Equals(activityId))
                executions.Add(this);

            foreach (var nestedExecution in Executions)
                nestedExecution.CollectExecutions(activityId, executions);
        }

        public virtual void CollectActiveActivityIds(IList<string> activeActivityIds)
        {
            if (IsActive && (Activity != null))
                activeActivityIds.Add(Activity.Id);

            foreach (var execution in Executions)
                execution.CollectActiveActivityIds(activeActivityIds);
        }


        // activity /////////////////////////////////////////////////////////////////

        /// <summary>
        ///     ensures initialization and returns the activity
        /// </summary>
        public virtual IPvmActivity Activity { get; set; }

        public virtual void ActivityInstanceStarting()
        {
            ActivityInstanceState = ActivityInstanceStateFields.Starting.StateCode;
        }


        public virtual void ActivityInstanceStarted()
        {
            ActivityInstanceState = ActivityInstanceStateFields.Default.StateCode;
        }

        public virtual void ActivityInstanceDone()
        {
            ActivityInstanceState = ActivityInstanceStateFields.Ending.StateCode;
        }

        protected internal abstract string GenerateActivityInstanceId(string activityId);


        /// <summary>
        ///     For a given target flow scope, this method returns the corresponding scope execution.
        ///     <para>
        ///         Precondition: the execution is active and executing an activity.
        ///         Can be invoked for scope and non scope executions.
        ///     </para>
        /// </summary>
        /// <param name="targetFlowScope"> scope activity or process definition for which the scope execution should be found </param>
        /// <returns> the scope execution for the provided targetFlowScope </returns>
        public virtual IActivityExecution FindExecutionForFlowScope(IPvmScope targetFlowScope)
        {
            // if this execution is not a scope execution, use the parent
            var scopeExecution = IsScope ? this : Parent;

            ScopeImpl currentActivity = (ScopeImpl) Activity;
            EnsureUtil.EnsureNotNull("activity of current execution", currentActivity);

            // if this is a scope execution currently executing a non scope activity
            currentActivity = currentActivity.IsScope ? currentActivity : currentActivity.FlowScope;

            return scopeExecution.FindExecutionForScope(currentActivity, (ScopeImpl) targetFlowScope);
        }


        public virtual IActivityExecution FindExecutionForScope(ScopeImpl currentScope, ScopeImpl targetScope)
        {
            if (!targetScope.IsScope)
                throw new ProcessEngineException("Target scope must be a scope.");

            var activityExecutionMapping = CreateActivityExecutionMapping(currentScope);
            var scopeExecution = activityExecutionMapping[targetScope];
            if (scopeExecution == null)
                scopeExecution = LegacyBehavior.GetScopeExecution(targetScope, activityExecutionMapping);
            return scopeExecution;
        }

        public virtual IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping(ScopeImpl currentScope)
        {
            if (!IsScope)
                throw new ProcessEngineException("Execution must be a scope execution");
            if (!currentScope.IsScope)
                throw new ProcessEngineException("Current scope must be a scope.");

            // A single path in the execution tree from a leaf (no child executions) to the root
            // may in fact contain multiple executions that correspond to leaves in the activity instance hierarchy.
            //
            // This is because compensation throwing executions have child executions. In that case, the
            // flow scope hierarchy is not aligned with the scope execution hierarchy: There is a scope
            // execution for a compensation-throwing event that is an ancestor of this execution,
            // while these events are not ancestor scopes of currentScope.
            //
            // The strategy to deal with this situation is as follows:
            // 1. Determine all executions that correspond to leaf activity instances
            // 2. Order the leaf executions in top-to-bottom fashion
            // 3. Iteratively build the activity execution mapping based on the leaves in top-to-bottom order
            //    3.1. For the first leaf, create the activity execution mapping regularly
            //    3.2. For every following leaf, rebuild the mapping but reuse any scopes and scope executions
            //         that are part of the mapping created in the previous iteration
            //
            // This process ensures that the resulting mapping does not contain scopes that are not ancestors
            // of currentScope and that it does not contain scope executions for such scopes.
            // For any execution hierarchy that does not involve compensation, the number of iterations in step 3
            // should be 1, i.e. there are no other leaf activity instance executions in the hierarchy.

            // 1. Find leaf activity instance executions
            var leafCollector = new LeafActivityInstanceExecutionCollector();
            new ExecutionWalker(this).AddPreVisitor(leafCollector)
                .WalkUntil();

            var leaves = leafCollector.Leaves;
            leaves.Remove(this);

            // 2. Order them from top to bottom
            leaves.Reverse();

            // 3. Iteratively extend the mapping for every additional leaf
            IDictionary<ScopeImpl, PvmExecutionImpl> mapping = new Dictionary<ScopeImpl, PvmExecutionImpl>();
            foreach (var leaf in leaves)
            {
                var leafFlowScope = leaf.FlowScope;
                var leafFlowScopeExecution = leaf.FlowScopeExecution;

                mapping = leafFlowScopeExecution.CreateActivityExecutionMapping(leafFlowScope, mapping);
            }

            // finally extend the mapping for the current execution
            // (note that the current execution need not be a leaf itself)
            mapping = CreateActivityExecutionMapping(currentScope, mapping);

            return mapping;
        }

        /// <summary>
        ///     Creates an extended mapping based on this execution and the given existing mapping.
        ///     Any entry <code>mapping</code> in mapping that corresponds to an ancestor scope of
        ///     <code>currentScope</code> is reused.
        /// </summary>
        public virtual IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping(
            ScopeImpl currentScope, IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
        {
            if (!IsScope)
                throw new ProcessEngineException("Execution must be a scope execution");
            if (!currentScope.IsScope)
                throw new ProcessEngineException("Current scope must be a scope.");

            // collect all ancestor scope executions unless one is encountered that is already in "mapping"
            var scopeExecutionCollector = new ScopeExecutionCollector();
            (new ExecutionWalker(this)).AddPreVisitor(scopeExecutionCollector)
                .WalkWhile(element => element == null || mapping.Values.Contains(element));
            var scopeExecutions = scopeExecutionCollector.ScopeExecutions;

            // collect all ancestor scopes unless one is encountered that is already in "mapping"
            var scopeCollector = new ScopeCollector();
            (new FlowScopeWalker(currentScope)).AddPreVisitor(scopeCollector)
                .WalkWhile(element => element == null || mapping.ContainsKey(element));

            var scopes = scopeCollector.Scopes;
            if (scopes.Count == 0)
            {
                throw new System.Exception("流程异常：scopes.Count=0,PvmExecutionImpl go:1522");
                var t = scopes;
            }
            // add all ancestor scopes and scopeExecutions that are already in "mapping"
            // and correspond to ancestors of the topmost previously collected scope
            var topMostScope = scopes[scopes.Count - 1];
            (new FlowScopeWalker(topMostScope.FlowScope)).AddPreVisitor(new TreeVisitorAnonymousInnerClass(this, mapping,
                    scopeExecutions, scopes))
                .WalkWhile();

            if (scopes.Count == scopeExecutions.Count)
            {
                // the trees are in sync
                IDictionary<ScopeImpl, PvmExecutionImpl> result = new Dictionary<ScopeImpl, PvmExecutionImpl>();
                for (var i = 0; i < scopes.Count; i++)
                    result[scopes[i]] = scopeExecutions[i];
                return result;
            }
            // Wounderful! The trees are out of sync. This is due to legacy behavior
            return LegacyBehavior.CreateActivityExecutionMapping(scopeExecutions, scopes);
        }

        // toString /////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            if (IsProcessInstanceExecution)
                return "ProcessInstance[" + ToStringIdentity + "]";
            return (IsConcurrent ? "IsConcurrent" : "") + (IsScope ? "IsScope" : "") + "Execution[" + ToStringIdentity +
                   "]";
        }

        /// <param name="targetScopeId"> - destination scope to be found in current execution tree </param>
        /// <returns> execution with activity id corresponding to targetScopeId </returns>
        protected internal virtual IActivityExecution FindExecutionForFlowScope(string targetScopeId)
        {
            EnsureUtil.EnsureNotNull("target scope id", targetScopeId);

            ScopeImpl currentActivity = (ScopeImpl) Activity;
            EnsureUtil.EnsureNotNull("activity of current execution", currentActivity);

            var walker = new FlowScopeWalker(currentActivity);
            var targetFlowScope = walker.WalkUntil(scope => scope == null || scope.Id.Equals(targetScopeId));

            if (targetFlowScope == null)
            {
                //throw LOG.scopeNotFoundException(targetScopeId, Id);
            }

            return FindExecutionForFlowScope(targetFlowScope);
        }


        public virtual void IncrementSequenceCounter()
        {
            SequenceCounter++;
        }

        public virtual bool IsInState(IActivityInstanceState state)
        {
            return ActivityInstanceState == state.StateCode;
        }

        public virtual void DisposeProcessInstanceStartContext()
        {
            StartContext = null;
        }

        public virtual void DisposeExecutionStartContext()
        {
            StartContext = null;
        }

        public virtual bool HasProcessInstanceStartContext()
        {
            return (StartContext != null) && StartContext is ProcessInstanceStartContext;
        }


        public virtual IActivityExecution GetParentScopeExecution(bool considerSuperExecution)
        {
            if (IsProcessInstanceExecution)
            {
                if (considerSuperExecution && (SuperExecution != null))
                {
                    var superExecution = (PvmExecutionImpl) SuperExecution;
                    if (superExecution.IsScope)
                        return superExecution;
                    return superExecution.Parent;
                }
                return null;
            }
            var parent = Parent;
            if (parent.IsScope)
                return parent;
            return parent.Parent;
        }

        /// <summary>
        /// Contains the delayed variable events, which will be dispatched on a save point.
        /// </summary>
        public IList<DelayedVariableEvent> DelayedEvents { get; protected internal set; } =
            new List<DelayedVariableEvent>();

        [NotMapped]
        public bool SkipIoMappings { get; set; }
        public virtual bool IsSuspended { get ; set ; }
        [JsonIgnore]
        public bool IsInSubProcess
        {
            get
            {
                return FlowScope.SubProcessScope;
            }
        }
        /// <summary>
        /// Delays a given variable event with the given target scope.
        /// </summary>
        /// <param name="targetScope">   the target scope of the variable event </param>
        /// <param name="variableEvent"> the variable event which should be delayed </param>
        public virtual void DelayEvent(PvmExecutionImpl targetScope, VariableEvent variableEvent)
        {
            DelayedVariableEvent delayedVariableEvent = new DelayedVariableEvent(targetScope, variableEvent);
            DelayEvent(delayedVariableEvent);
        }

        /// <summary>
        /// Delays and stores the given DelayedVariableEvent on the process instance.
        /// </summary>
        /// <param name="delayedVariableEvent"> the DelayedVariableEvent which should be store on the process instance </param>
        public virtual void DelayEvent(DelayedVariableEvent delayedVariableEvent)
        {

            //if process definition has no conditional events the variable events does not have to be delayed
            bool? hasConditionalEvents = this.ProcessDefinition.Properties.Get(BpmnProperties.HasConditionalEvents);
            if (hasConditionalEvents == null || !hasConditionalEvents.Equals(true))
            {
                return;
            }

            if (IsProcessInstanceExecution)
            {
                DelayedEvents.Add(delayedVariableEvent);
            }
            else
            {
                ((PvmExecutionImpl) ProcessInstance).DelayEvent(delayedVariableEvent);
            }
        }

        /// <summary>
        ///     Contains the delayed variable events, which will be dispatched on a save point.
        /// </summary>
        /// <summary>
        ///     Delays a given variable event with the given target scope.
        /// </summary>
        /// <param name="targetScope">   the target scope of the variable event </param>
        /// <param name="variableEvent"> the variable event which should be delayed </param>
        /// <summary>
        ///     Delays and stores the given DelayedVariableEvent on the process instance.
        /// </summary>
        /// <param name="delayedVariableEvent"> the DelayedVariableEvent which should be store on the process instance </param>
        /// <summary>
        ///     The current delayed variable events.
        /// </summary>
        /// <returns> a list of DelayedVariableEvent objects </returns>
        /// <summary>
        ///     Cleares the current delayed variable events.
        /// </summary>
        public virtual void ClearDelayedEvents()
        {
            if (IsProcessInstanceExecution)
            {
                DelayedEvents.Clear();
            }
            else
            {
                ((PvmExecutionImpl) ProcessInstance).ClearDelayedEvents();
            }
        }


        /// <summary>
        ///     Dispatches the current delayed variable events and performs the given atomic operation
        ///     if the current state was not changed.
        /// </summary>
        /// <param name="atomicOperation"> the atomic operation which should be executed </param>
        public virtual void DispatchDelayedEventsAndPerformOperation(IPvmAtomicOperation atomicOperation)
        {
            DispatchDelayedEventsAndPerformOperation((e) => e.PerformOperation(atomicOperation));
        }

        /// <summary>
        ///     Dispatches the current delayed variable events and performs the given atomic operation
        ///     if the current state was not changed.
        /// </summary>
        /// <param name="continuation"> the atomic operation continuation which should be executed </param>
        public virtual void DispatchDelayedEventsAndPerformOperation(Action<PvmExecutionImpl> continuation)
        {
            var execution = this;

            if (execution.DelayedEvents.Count == 0)
            {
                ContinueExecutionIfNotCanceled(continuation, execution);
                return;
            }

            ContinueIfExecutionDoesNotAffectNextOperation((e) =>
                {
                    e.DispatchScopeEvents(execution);
                },
                (e) =>
                {
                    e.ContinueExecutionIfNotCanceled(continuation, execution);
                }
                , execution);
        }

        /// <summary>
        ///     Executes the given depending operations with the given execution.
        ///     The execution state will be checked with the help of the activity instance id and activity id of the execution
        ///     before and after
        ///     the dispatching callback call. If the id's are not changed the
        ///     continuation callback is called.
        /// </summary>
        /// <param name="dispatching">         the callback to dispatch the variable events </param>
        /// <param name="continuation">        the callback to continue with the next atomic operation </param>
        /// <param name="execution">           the execution which is used for the execution </param>
        public virtual void ContinueIfExecutionDoesNotAffectNextOperation(
            Action<PvmExecutionImpl> dispatching, Action<PvmExecutionImpl> continuation,
            PvmExecutionImpl execution)
        {
            var lastActivityId = execution.ActivityId;
            var lastActivityInstanceId = GetActivityInstanceId(execution);

            dispatching(execution);

            execution = (PvmExecutionImpl) (execution.ReplacedBy != null ? execution.ReplacedBy : execution);
            var currentActivityInstanceId = GetActivityInstanceId(execution);
            var currentActivityId = execution.ActivityId;

            //if execution was canceled or was changed during the dispatch we should not execute the next operation
            //since another atomic operation was executed during the dispatching
            if (!execution.Canceled &&
                IsOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId, currentActivityId))
                continuation(execution);
        }

        protected internal virtual void ContinueExecutionIfNotCanceled(Action<PvmExecutionImpl> continuation,
            PvmExecutionImpl execution)
        {
            if ((continuation != null) && !execution.Canceled)
                continuation(execution);
        }

        /// <summary>
        /// Dispatches the delayed variable event, if the target scope and replaced by scope (if target scope was replaced) have the
        /// same activity Id's and activity instance id's.
        /// </summary>
        /// <param name="targetScope">          the target scope on which the event should be dispatched </param>
        /// <param name="replacedBy">           the replaced by pointer which should have the same state </param>
        /// <param name="activityIds">          the map which maps scope to activity id </param>
        /// <param name="activityInstanceIds">  the map which maps scope to activity instance id </param>
        /// <param name="delayedVariableEvent"> the delayed variable event which should be dispatched </param>
        private void DispatchOnSameActivity(PvmExecutionImpl targetScope, IActivityExecution replacedBy,
            IDictionary<PvmExecutionImpl, string> activityIds, IDictionary<PvmExecutionImpl, string> activityInstanceIds,
            DelayedVariableEvent delayedVariableEvent)
        {
            //check if the target scope has the same activity id and activity instance id
            //since the dispatching was started
            string currentActivityInstanceId = GetActivityInstanceId(targetScope);
            string currentActivityId = targetScope.ActivityId;

            string lastActivityInstanceId = activityInstanceIds[targetScope];
            string lastActivityId = activityIds[targetScope];

            bool onSameAct = IsOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId,
                currentActivityId);

            //If not we have to check the replace pointer,
            //which was set if a concurrent execution was created during the dispatching.
            if (targetScope != replacedBy && !onSameAct)
            {
                currentActivityInstanceId = GetActivityInstanceId(replacedBy);
                currentActivityId = replacedBy.ActivityId;
                onSameAct = IsOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId,
                    currentActivityId);
            }

            //dispatching
            if (onSameAct && IsOnDispatchableState(targetScope))
            {
                targetScope.DispatchEvent(delayedVariableEvent.Event);
            }
        }

        /// <summary>
        ///     Dispatches the current delayed variable events on the scope of the given execution.
        /// </summary>
        /// <param name="execution"> the execution on which scope the delayed variable should be dispatched </param>
        /// <summary>
        ///     Initializes the given maps with the target scopes and current activity id's and activity instance id's.
        /// </summary>
        /// <param name="delayedEvents">       the delayed events which contains the information about the target scope </param>
        /// <param name="activityInstanceIds"> the map which maps target scope to activity instance id </param>
        /// <param name="activityIds">         the map which maps target scope to activity id </param>
        /// <summary>
        ///     Dispatches the delayed variable event, if the target scope and replaced by scope (if target scope was replaced)
        ///     have the
        ///     same activity Id's and activity instance id's.
        /// </summary>
        /// <param name="targetScope">          the target scope on which the event should be dispatched </param>
        /// <param name="replacedBy">           the replaced by pointer which should have the same state </param>
        /// <param name="activityIds">          the map which maps scope to activity id </param>
        /// <param name="activityInstanceIds">  the map which maps scope to activity instance id </param>
        /// <param name="delayedVariableEvent"> the delayed variable event which should be dispatched </param>
        /// <summary>
        ///     Checks if the given execution is on a dispatchable state.
        ///     That means if the current activity is not a leaf in the activity tree OR
        ///     it is a leaf but not a scope OR it is a leaf, a scope
        ///     and the execution is in state DEFAULT, which means not in state
        ///     Starting, Execute or Ending. For this states it is
        ///     prohibited to trigger conditional events, otherwise unexpected behavior can appear.
        /// </summary>
        /// <returns> true if the execution is on a dispatchable state, false otherwise </returns>
        private bool IsOnDispatchableState(PvmExecutionImpl targetScope)
        {
            var targetActivity = targetScope.Activity;
            return ReferenceEquals(targetScope.ActivityId, null) || !targetActivity.IsScope ||
                   targetScope.IsInState(ActivityInstanceStateFields.Default);
            //if not leaf, activity id is null -> dispatchable
            // if leaf and not scope -> dispatchable
            // if leaf, scope and state in default -> dispatchable
        }


        /// <summary>
        ///     Compares the given activity instance id's and activity id's to check if the execution is on the same
        ///     activity as before an operation was executed. The activity instance id's can be null on transitions.
        ///     In this case the activity Id's have to be equal, otherwise the execution changed.
        /// </summary>
        /// <param name="lastActivityInstanceId">    the last activity instance id </param>
        /// <param name="lastActivityId">            the last activity id </param>
        /// <param name="currentActivityInstanceId"> the current activity instance id </param>
        /// <param name="currentActivityId">         the current activity id </param>
        /// <returns> true if the execution is on the same activity, otherwise false </returns>
        private bool IsOnSameActivity(string lastActivityInstanceId, string lastActivityId,
            string currentActivityInstanceId, string currentActivityId)
        {
            return (ReferenceEquals(lastActivityInstanceId, null) &&
                    ReferenceEquals(lastActivityInstanceId, currentActivityInstanceId) &&
                    lastActivityId.Equals(currentActivityId)) ||
                   (!ReferenceEquals(lastActivityInstanceId, null) &&
                    lastActivityInstanceId.Equals(currentActivityInstanceId) &&
                    (ReferenceEquals(lastActivityId, null) || lastActivityId.Equals(currentActivityId)));
            //ActivityInstanceId's can be null on transitions, so the activityId must be equal
            //if ActivityInstanceId's are not null they must be equal -> otherwise execution changed
        }

        /// <summary>
        ///     Returns the activity instance id for the given execution.
        /// </summary>
        /// <param name="targetScope"> the execution for which the activity instance id should be returned </param>
        /// <returns> the activity instance id </returns>
        private string GetActivityInstanceId(IActivityExecution targetScope)
        {
            if (targetScope.IsConcurrent)
                return targetScope.ActivityInstanceId;
            var targetActivity = targetScope.Activity;
            if ((targetActivity != null) && (targetActivity.Activities.Count == 0))
                return targetScope.ActivityInstanceId;
            return targetScope.ParentActivityInstanceId;
        }

        /// <summary>
        /// Dispatches the current delayed variable events on the scope of the given execution.
        /// </summary>
        /// <param name="execution"> the execution on which scope the delayed variable should be dispatched </param>
        protected internal virtual void DispatchScopeEvents(IActivityExecution execution)
        {
            IActivityExecution scopeExecution = execution.IsScope ? execution : execution.Parent;

            IList<DelayedVariableEvent> delayedEvents = new List<DelayedVariableEvent>(scopeExecution.DelayedEvents);
            scopeExecution.ClearDelayedEvents();

            IDictionary<PvmExecutionImpl, string> activityInstanceIds = new Dictionary<PvmExecutionImpl, string>();
            IDictionary<PvmExecutionImpl, string> activityIds = new Dictionary<PvmExecutionImpl, string>();
            InitActivityIds(delayedEvents, activityInstanceIds, activityIds);

            //For each delayed variable event we have to check if the delayed event can be dispatched,
            //the check will be done with the help of the activity id and activity instance id.
            //That means it will be checked if the dispatching changed the execution tree in a way that we can't dispatch the
            //the other delayed variable events. We have to check the target scope with the last activity id and activity instance id
            //and also the replace pointer if it exist. Because on concurrency the replace pointer will be set on which we have
            //to check the latest state.
            foreach (DelayedVariableEvent @event in delayedEvents)
            {
                PvmExecutionImpl targetScope = @event.TargetScope;
                IActivityExecution replaced = targetScope.ReplacedBy != null ? targetScope.ReplacedBy : targetScope;
                DispatchOnSameActivity(targetScope, replaced, activityIds, activityInstanceIds, @event);
            }
        }

        /// <summary>
        /// Initializes the given maps with the target scopes and current activity id's and activity instance id's.
        /// </summary>
        /// <param name="delayedEvents">       the delayed events which contains the information about the target scope </param>
        /// <param name="activityInstanceIds"> the map which maps target scope to activity instance id </param>
        /// <param name="activityIds">         the map which maps target scope to activity id </param>
        protected internal virtual void InitActivityIds(IList<DelayedVariableEvent> delayedEvents,
            IDictionary<PvmExecutionImpl, string> activityInstanceIds, IDictionary<PvmExecutionImpl, string> activityIds)
        {

            foreach (DelayedVariableEvent @event in delayedEvents)
            {
                PvmExecutionImpl targetScope = @event.TargetScope;

                string targetScopeActivityInstanceId = GetActivityInstanceId(targetScope);
                activityInstanceIds[targetScope] = targetScopeActivityInstanceId;
                activityIds[targetScope] = targetScope.ActivityId;
            }
        }

        /// <summary>
        /// Returns the newest incident in this execution
        /// </summary>
        /// <param name="incidentType"> the type of new incident </param>
        /// <param name="configuration"> configuration of the incident </param>
        /// <returns> new incident </returns>
        public IIncident CreateIncident(string incidentType, string configuration)
        {
            return CreateIncident(incidentType, configuration, null);
        }

        public virtual IIncident CreateIncident(string incidentType, string configuration, string message)
        {
            IncidentContext incidentContext = new IncidentContext();

            incidentContext.TenantId = this.TenantId;
            incidentContext.ProcessDefinitionId = this.ProcessDefinitionId;
            incidentContext.ExecutionId = this.Id;
            incidentContext.ActivityId = this.ActivityId;
            incidentContext.Configuration = configuration;

            IncidentEntity newIncident = null;
            IIncidentHandler incidentHandler = FindCustomIncidentHandler(incidentType);

            if (incidentHandler == null)
            {
                newIncident = IncidentEntity.CreateAndInsertIncident(incidentType, incidentContext, message);
                newIncident.CreateRecursiveIncidents();
            }
            else
            {
                incidentHandler.HandleIncident(incidentContext, message);
                newIncident = FindLastIncident();
            }
            return newIncident;
        }

        public abstract IncidentEntity FindLastIncident();

        /// <summary>
        /// Resolves an incident with given id.
        /// </summary>
        /// <param name="incidentId"> </param>
        public void ResolveIncident(string incidentId)
        {
            IncidentEntity incident = (IncidentEntity)Context.CommandContext.IncidentManager.FindIncidentById(incidentId);

            IIncidentHandler incidentHandler = FindCustomIncidentHandler(incident.IncidentType);

            if (incidentHandler == null)
            {
                incident.Resolve();
            }
            else
            {
                IncidentContext incidentContext = new IncidentContext();
                incidentContext.TenantId = incident.TenantId;
                incidentContext.ActivityId = incident.ActivityId;
                incidentContext.Configuration = incident.Configuration;
                incidentContext.JobDefinitionId = incident.JobDefinitionId;
                incidentContext.ProcessDefinitionId = incident.ProcessDefinitionId;
                incidentContext.ExecutionId = incident.ExecutionId;
                incidentHandler.ResolveIncident(incidentContext);
            }
        }

        public virtual IIncidentHandler FindCustomIncidentHandler(string incidentType)
        {
            IDictionary<string, IIncidentHandler> incidentHandlers = Context.ProcessEngineConfiguration.IncidentHandlers;
            return incidentHandlers[incidentType];
        }

        public virtual IList<IActivityExecution> GetExecutionsEntity()
        {
            throw new NotImplementedException();
        }

        public virtual void AddExecutionObserver(IExecutionObserver observer)
        {
            throw new NotImplementedException();
        }

        private class TreeVisitorAnonymousInnerClass : ITreeVisitor<ScopeImpl>
        {
            private readonly PvmExecutionImpl outerInstance;

            private readonly IDictionary<ScopeImpl, PvmExecutionImpl> mapping;
            private readonly IList<PvmExecutionImpl> scopeExecutions;
            private readonly IList<ScopeImpl> scopes;

            public TreeVisitorAnonymousInnerClass(PvmExecutionImpl outerInstance,
                IDictionary<ScopeImpl, PvmExecutionImpl> mapping, IList<PvmExecutionImpl> scopeExecutions,
                IList<ScopeImpl> scopes)
            {
                this.outerInstance = outerInstance;
                this.mapping = mapping;
                this.scopeExecutions = scopeExecutions;
                this.scopes = scopes;
            }

            public virtual void Visit(ScopeImpl obj)
            {
                scopes.Add(obj);
                PvmExecutionImpl priorMappingExecution;
                mapping.TryGetValue(obj, out priorMappingExecution);

                if (priorMappingExecution != null && !scopeExecutions.Contains(priorMappingExecution))
                {
                    scopeExecutions.Add(priorMappingExecution);
                }
            }
        }
    }
}