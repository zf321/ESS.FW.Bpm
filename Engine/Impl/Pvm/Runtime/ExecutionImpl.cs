using System;
using System.Collections.Generic;
using System.Threading;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ExecutionImpl : PvmExecutionImpl
    {

        //private static AtomicInteger idGenerator = new AtomicInteger();

        // current position /////////////////////////////////////////////////////////

        ///// <summary>
        /////     the process instance.  this is the root of the execution tree.
        /////     the processInstance of a process instance is a self reference.
        ///// </summary>
        //protected internal ExecutionImpl processInstance;

        ///// <summary>
        /////     reference to a subprocessinstance, not-null if currently subprocess is started from this execution
        ///// </summary>
        //protected internal ExecutionImpl subProcessInstance;

        ///// <summary>
        /////     super execution, not-null if this execution is part of a subprocess
        ///// </summary>
        //protected internal ExecutionImpl superExecution;

        /// <summary>
        ///     super case execution, not-null if this execution is part of a case execution
        /// </summary>
        /// <summary>
        ///     reference to a subcaseinstance, not-null if currently subcase is started from this execution
        /// </summary>
        //protected internal CaseExecutionImpl subCaseInstance;

        // variables/////////////////////////////////////////////////////////////////
        protected internal VariableStore variableStore =
            new VariableStore();

        // lifecycle methods ////////////////////////////////////////////////////////

        public override IActivityExecution ParentExecution
        {
            get { return  Parent; }
            set { Parent =  (ExecutionImpl) value; }
        }
        

        // executions ///////////////////////////////////////////////////////////////

        public override IList<IActivityExecution> ExecutionsAsCopy
        {
            get { return (Executions); }
        }

        protected internal override string ToStringIdentity => Convert.ToString((this).GetHashCode());

        // getters and setters //////////////////////////////////////////////////////
        protected internal override VariableStore VariableStore => variableStore;

        protected internal override IVariableInstanceFactory<ICoreVariableInstance> VariableInstanceFactory => VariableInstanceEntityFactory.Instance;

        protected override IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> VariableInstanceLifecycleListeners => new List<IVariableInstanceLifecycleListener<ICoreVariableInstance>>();
        

        /// <summary>
        ///     creates a new execution. properties processDefinition, processInstance and activity will be initialized.
        /// </summary>
        public override IActivityExecution CreateExecution(bool initializeExecutionStartContext)
        {
            // create the new child execution
            var createdExecution = (ExecutionImpl) NewExecution();

            // initialize sequence counter
            createdExecution.SequenceCounter = SequenceCounter;

            // manage the bidirectional parent-child relation
            createdExecution.SetParent(this);

            // initialize the new execution
            createdExecution.ProcessDefinition = ProcessDefinition;
            createdExecution.ProcessInstance=(ProcessInstance);
            createdExecution.Activity = Activity;

            // make created execution start in same activity instance
            createdExecution.ActivityInstanceId = ActivityInstanceId;

            if (initializeExecutionStartContext)
                createdExecution.StartContext = new ExecutionStartContext();
            else if (StartContext != null)
                createdExecution.StartContext = StartContext;

            createdExecution.SkipCustomListeners = SkipCustomListeners;
            createdExecution.SkipIoMapping = SkipIoMapping;

            return createdExecution;
        }

        // public override void setSubCaseInstance(CmmnExecution subCaseInstance)
        // {
        //this.subCaseInstance = (CaseExecutionImpl) subCaseInstance;
        // }

        // public override CaseExecutionImpl createSubCaseInstance(CmmnCaseDefinition caseDefinition)
        // {
        //return createSubCaseInstance(caseDefinition, null);
        // }

        // public override CaseExecutionImpl createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey)
        // {
        //CaseExecutionImpl caseInstance = (CaseExecutionImpl) caseDefinition.createCaseInstance(businessKey);

        //// manage bidirectional super-process-sub-case-instances relation
        //subCaseInstance.setSuperExecution(this);
        //setSubCaseInstance(subCaseInstance);

        //return caseInstance;
        // }

        // process definition ///////////////////////////////////////////////////////

        public override string ProcessDefinitionId { get { return ProcessDefinition.Id; } set { ProcessDefinition.Id = value; } } 

        public override string ProcessInstanceId
        {
            get { return ProcessInstance.Id; }
            set { ProcessInstance.Id = value; }
        }
        public override string BusinessKey
        {
            get { return ProcessInstance.BusinessKey; }
            set { ProcessInstance.BusinessKey = value; }
        }

        public override string ProcessBusinessKey => ProcessInstance.BusinessKey;

        //allow for subclasses to expose a real id /////////////////////////////////

        public override string Id => (this).GetHashCode().ToString();


        public override string CurrentActivityName
        {
            get
            {
                string currentActivityName = null;
                if (Activity != null)
                    currentActivityName = (string) Activity.GetProperty("name");
                return currentActivityName;
            }
        }

        public override IFlowElement BpmnModelElementInstance
        {
            get
            {
                throw new NotSupportedException(typeof(IBpmnModelExecutionContext).FullName +
                                                " is unsupported in transient ExecutionImpl");
            }
        }

        public override IBpmnModelInstance BpmnModelInstance
        {
            get
            {
                throw new NotSupportedException(typeof(IBpmnModelExecutionContext).FullName +
                                                " is unsupported in transient ExecutionImpl");
            }
        }

        public override IProcessEngineServices ProcessEngineServices
        {
            get
            {
                throw new NotSupportedException(typeof(IProcessEngineServicesAware).FullName +
                                                " is unsupported in transient ExecutionImpl");
            }
        }


        public override void ForceUpdate()
        {
            // nothing to do
        }

        // process instance /////////////////////////////////////////////////////////

        public override void Start(IDictionary<string, object> variables)
        {
            if (IsProcessInstanceExecution)
                if (StartContext == null)
                    StartContext = new ProcessInstanceStartContext((ActivityImpl) ProcessDefinition.Initial);

            base.Start(variables);
        }

        /// <summary>
        ///     instantiates a new execution.  can be overridden by subclasses
        /// </summary>
        protected internal override PvmExecutionImpl NewExecution()
        {
            return new ExecutionImpl();
        }

        public override void Initialize()
        {
        }

        public override void InitializeTimerDeclarations()
        {
        }
        


        // activity /////////////////////////////////////////////////////////////////
        private static int nextId = 0;
        /// <summary>
        ///     generates an activity instance id
        /// </summary>
        protected internal override string GenerateActivityInstanceId(string activityId)
        {

            Interlocked.Increment(ref nextId);
            var compositeId = activityId + ":" + nextId;
            if (compositeId.Length > 64)
            {
                return nextId.ToString();
            }
            return compositeId;
        }

        //toString /////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            if (IsProcessInstanceExecution)
            {
                return "ProcessInstance[" + ToStringIdentity + "]";
            }
            return (IsEventScope ? "EventScope" : "") + (IsConcurrent ? "Concurrent" : "") + (IsScope ? "Scope" : "") +
                   "Execution[" + ToStringIdentity + "]";
        }

        public override void FireHistoricProcessStartEvent()
        {
            // do nothing
        }

        protected internal override void RemoveVariablesLocalInternal()
        {
            // do nothing
        }

        public override void SetParentExecution(IActivityExecution parent)
        {
            this.Parent = parent;
        }

        public override IActivityExecution ReplacedBy { get; set; }

        private ExecutionImpl _subProcessInstance;
        public override IActivityExecution SubProcessInstance { get => _subProcessInstance;
            set => _subProcessInstance = (ExecutionImpl) value;
        }

        public override IncidentEntity FindLastIncident()
        {
            throw new NotSupportedException();
        }
    }
}