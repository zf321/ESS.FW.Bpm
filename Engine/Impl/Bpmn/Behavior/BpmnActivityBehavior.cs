using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Helper class for implementing BPMN 2.0 activities, offering convenience
    ///     methods specific to BPMN 2.0.
    ///     This class can be used by inheritance or aggregation.
    ///     
    /// </summary>
    public class BpmnActivityBehavior
    {
        protected internal static BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        /// <summary>
        ///     Performs the default outgoing BPMN 2.0 behavior, which is having parallel
        ///     paths of executions for the outgoing sequence flow.
        ///     More precisely: every sequence flow that has a condition which evaluates to
        ///     true (or which doesn't have a condition), is selected for continuation of
        ///     the process instance. If multiple sequencer flow are selected, multiple,
        ///     parallel paths of executions are created.
        /// </summary>
        public virtual void PerformDefaultOutgoingBehavior(IActivityExecution activityExceution)
        {
            PerformOutgoingBehavior(activityExceution, true, false, null);
        }

        /// <summary>
        ///     Performs the default outgoing BPMN 2.0 behavior (@see
        ///     <seealso cref="#performDefaultOutgoingBehavior(ActivityExecution)" />), but without
        ///     checking the conditions on the outgoing sequence flow.
        ///     This means that every outgoing sequence flow is selected for continuing the
        ///     process instance, regardless of having a condition or not. In case of
        ///     multiple outgoing sequence flow, multiple parallel paths of executions will
        ///     be created.
        /// </summary>
        public virtual void PerformIgnoreConditionsOutgoingBehavior(IActivityExecution activityExecution)
        {
            PerformOutgoingBehavior(activityExecution, false, false, null);
        }

        /// <summary>
        ///     Actual implementation of leaving an activity.
        /// </summary>
        /// <param name="execution">
        ///     The current execution context
        /// </param>
        /// <param name="checkConditions">
        ///     Whether or not to check conditions before determining whether or
        ///     not to take a transition.
        /// </param>
        /// <param name="throwExceptionIfExecutionStuck">
        ///     If true, an <seealso cref="ProcessEngineException" /> will be thrown in case no
        ///     transition could be found to leave the activity.
        /// </param>
        protected internal virtual void PerformOutgoingBehavior(IActivityExecution execution, bool checkConditions,
            bool throwExceptionIfExecutionStuck, IList<IActivityExecution> reusableExecutions)
        {
            Log.LeavingActivity(execution.Activity.Id);

            var defaultSequenceFlow = (string)execution.Activity.GetProperty("default");
            IList<IPvmTransition> transitionsToTake = new List<IPvmTransition>();

            IList<IPvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;
            foreach (IPvmTransition outgoingTransition in outgoingTransitions)
            {
                if (defaultSequenceFlow == null || outgoingTransition.Id!=defaultSequenceFlow)
                {
                    var condition = (ICondition) outgoingTransition.GetProperty(BpmnParse.PropertynameCondition);
                    if ((condition == null) || !checkConditions || condition.Evaluate(execution))
                        transitionsToTake.Add(outgoingTransition);
                }
            }

            if (transitionsToTake.Count == 1)
            {
                execution.LeaveActivityViaTransition(transitionsToTake.ElementAt(0));
            }
            else if (transitionsToTake.Count >= 1)
            {
                if (reusableExecutions == null || reusableExecutions.Count == 0)
                {
                    execution.LeaveActivityViaTransitions(transitionsToTake,new List<IActivityExecution>() { execution });
                }
                else
                {
                    execution.LeaveActivityViaTransitions(transitionsToTake, reusableExecutions);
                }
            }
            else
            {
                //if (!ReferenceEquals(defaultSequenceFlow, null))
                if(!string.IsNullOrEmpty(defaultSequenceFlow))
                {
                    var defaultTransition = execution.Activity.FindOutgoingTransition(defaultSequenceFlow);
                    if (defaultTransition != null)
                        execution.LeaveActivityViaTransition(defaultTransition);
                    else
                        throw Log.MissingDefaultFlowException(execution.Activity.Id, defaultSequenceFlow);
                }
                else if (outgoingTransitions.Count > 0)
                {
                    throw Log.MissingConditionalFlowException(execution.Activity.Id);
                }
                else
                {
                    
                    if (((ActivityImpl)execution.Activity).CompensationHandler &&
                        IsAncestorCompensationThrowing(execution))
                    {
                        execution.EndCompensation();
                    }
                    else
                    {
                        Log.MissingOutgoingSequenceFlow(execution.Activity.Id);
                        execution.End(true);

                        if (throwExceptionIfExecutionStuck)
                            throw Log.StuckExecutionException(execution.Activity.Id);
                    }
                }
            }
        }

        protected internal virtual bool IsAncestorCompensationThrowing(IActivityExecution execution)
        {
            var parent = execution.Parent;
            while (parent != null)
            {
                if (CompensationBehavior.IsCompensationThrowing((PvmExecutionImpl)parent))
                    return true;
                parent = parent.Parent;
            }
            return false;
        }
    }
}