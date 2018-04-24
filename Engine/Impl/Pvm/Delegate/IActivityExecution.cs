using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{

    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface IActivityExecution : IDelegateExecution, IPvmProcessInstance
    {
        /* Process instance/activity/transition retrieval */

        new string Id { get; set; }
        /// <summary>
        ///     <para>Creates a new sub case instance.</para>
        ///     <para>
        ///         <code>This</code> execution will be the super execution of the
        ///         created sub case instance.
        ///     </para>
        /// </summary>
        /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition" /> of the sub case instance. </param>
        /// <summary>
        ///     <para>Creates a new sub case instance.</para>
        ///     <para>
        ///         <code>This</code> execution will be the super execution of the
        ///         created sub case instance.
        ///     </para>
        /// </summary>
        /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition" /> of the sub case instance. </param>
        /// <param name="businessKey"> The businessKey to be set on sub case instance. </param>
        /// <summary>
        ///     returns the parent of this execution, or null if there no parent.
        /// </summary>
        IActivityExecution Parent { get; set; }

        /// <summary>
        ///     returns the list of execution of which this execution the parent of.
        ///     This is a copy of the actual list, so a modification has no direct effect.
        /// </summary>
        IList<IActivityExecution> Executions { get; }

        /// <summary>
        ///     returns child executions that are not event scope executions.
        /// </summary>
        IList<IActivityExecution> NonEventScopeExecutions { get; }

        /* State management */

        /// <summary>
        ///     makes this execution active or inactive.
        /// </summary>
        bool IsActive { set; get; }



        /// <summary>
        ///     changes the concurrent indicator on this execution.
        /// </summary>
        bool IsConcurrent { set; get; }


        /// <summary>
        ///     returns whether this execution is a process instance or not.
        /// </summary>
        bool IsProcessInstanceExecution { get; }

        /// <summary>
        ///     Returns whether this execution is a scope.
        /// </summary>
        bool IsScope { get; set; }


        /// <summary>
        ///     Returns whether this execution completed the parent scope.
        /// </summary>
        bool IsCompleteScope { get; }

        bool IsInSubProcess { get; }
        /// <summary>
        ///     An activity which is to be started next.
        /// </summary>
        IPvmActivity NextActivity { get; }

        IPvmTransition Transition { get; set; }
        IActivityExecution ReplacedBy { get; set; }
        long SequenceCounter { get; set; }
        bool SkipCustomListeners { get; set; }
        bool SkipIoMapping { get; set; }
        IList<IActivityExecution> ExecutionsAsCopy { get; }
        IList<IPvmTransition> TransitionsToTake { get; set; }
        IActivityExecution FlowScopeExecution { get; }
        IActivityExecution SubProcessInstance { get; set; }
        IList<IActivityExecution> AllChildExecutions { get;  }
        bool IsEventScope { get; set; }
        IList<DelayedVariableEvent> DelayedEvents { get;  }
        string ActivityId { get; }
        bool IsDeleteRoot { get; set; }
        bool SkipIoMappings { get; set; }
        ExecutionStartContext ExecutionStartContext { get; }

        /// <summary>
        ///     invoked to notify the execution that a new activity instance is started
        /// </summary>
        void EnterActivityInstance();

        /// <summary>
        ///     invoked to notify the execution that an activity instance is ended.
        /// </summary>
        void LeaveActivityInstance();

        /* Execution management */

        /// <summary>
        ///     creates a new execution. This execution will be the parent of the newly created execution.
        ///     properties processDefinition, processInstance and activity will be initialized.
        /// </summary>
        IActivityExecution CreateExecution();

        /// <summary>
        ///     creates a new execution. This execution will be the parent of the newly created execution.
        ///     properties processDefinition, processInstance and activity will be initialized.
        /// </summary>
        IActivityExecution CreateExecution(bool initializeExecutionStartContext);

        /// <summary>
        ///     creates a new sub process instance.
        ///     The current execution will be the super execution of the created execution.
        /// </summary>
        /// <param name="processDefinition"> The <seealso cref="IPvmProcessDefinition" /> of the subprocess. </param>
        IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition);

        /// <seealso cref= # createSubProcessInstance( PvmProcessDefinition
        /// )
        /// </seealso>
        /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition" /> of the subprocess. </param>
        /// <param name="businessKey"> the business key of the process instance </param>
        IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition, string businessKey);

        /// <seealso cref= # createSubProcessInstance( PvmProcessDefinition
        /// )
        /// </seealso>
        /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition" /> of the subprocess. </param>
        /// <param name="businessKey"> the business key of the process instance </param>
        /// <param name="caseInstanceId"> the case instance id of the process instance </param>
        IPvmProcessInstance CreateSubProcessInstance(IPvmProcessDefinition processDefinition, string businessKey,
            string caseInstanceId);

        /// <returns> true if this execution has child executions (event scope executions or not) </returns>
        bool HasChildren();

        /// <summary>
        ///     ends this execution.
        /// </summary>
        void End(bool isScopeComplete);

        /// <summary>
        ///     Execution finished compensation. Removes this
        ///     execution and notifies listeners.
        /// </summary>
        void EndCompensation();

        /// <summary>
        ///     Inactivates this execution.
        ///     This is useful for example in a join: the execution
        ///     still exists, but it is not longer active.
        /// </summary>
        void InActivate();

        /// <summary>
        ///     Retrieves all executions which are concurrent and inactive at the given activity.
        /// </summary>
        IList<IActivityExecution> FindInactiveConcurrentExecutions(IPvmActivity activity);

        IList<IActivityExecution> FindInactiveChildExecutions(IPvmActivity activity);

        /// <summary>
        ///     Takes the given outgoing transitions, and potentially reusing
        ///     the given list of executions that were previously joined.
        /// </summary>
        void LeaveActivityViaTransitions(IList<IPvmTransition> outgoingTransitions, IList<IActivityExecution> joinedExecutions);

        void LeaveActivityViaTransition(IPvmTransition outgoingTransition);

        /// <summary>
        ///     Executes the <seealso cref="IActivityBehavior" /> associated with the given activity.
        /// </summary>
        void ExecuteActivity(IPvmActivity activity);

        /// <summary>
        ///     Called when an execution is interrupted. This will remove all associated entities
        ///     such as event subscriptions, jobs, ...
        /// </summary>
        void Interrupt(string reason);


        void Remove();
        void Destroy();

        bool TryPruneLastConcurrentChild();

        void ForceUpdate();

        /// <summary>
        ///     Assumption: the current execution is active and executing an activity (<seealso cref="#getActivity()" /> is not
        ///     null).
        ///     For a given target scope, this method returns the scope execution.
        /// </summary>
        /// <param name="targetScope">
        ///     scope activity or process definition for which the scope execution should be found;
        ///     must be an ancestor of the execution's current activity
        ///     @return
        /// </param>
        IActivityExecution FindExecutionForFlowScope(IPvmScope targetScope);

        /// <summary>
        ///     Returns a mapping from scope activities to scope executions for all scopes that
        ///     are ancestors of the activity currently executed by this execution.
        ///     Assumption: the current execution is active and executing an activity (<seealso cref="#getActivity()" /> is not
        ///     null).
        /// </summary>
        IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping();

        void DeleteCascade(string reason, bool skipCustomListeners, bool skipIoMappings);
        void CollectExecutions(string activityId, IList<IPvmExecution> executions);
        void CollectActiveActivityIds(IList<string> activeActivityIds);
        IActivityExecution FindExecutionForScope(ScopeImpl currentActivity, ScopeImpl targetFlowScope);
        void ClearDelayedEvents();
        IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping(ScopeImpl flowScope);
        IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping(ScopeImpl leafFlowScope, IDictionary<ScopeImpl, PvmExecutionImpl> mapping);
        void Take();
        IActivityExecution CreateConcurrentExecution();
        void ExecuteActivities(IList<IPvmActivity> activityStack, IPvmActivity targetActivity, IPvmTransition targetTransition, IDictionary<string, object> variables, IDictionary<string, object> localVariables, bool skipCustomListeners, bool skipIoMappings);
        void DisposeExecutionStartContext();
        IList<IActivityExecution> GetExecutionsEntity();
    }
}