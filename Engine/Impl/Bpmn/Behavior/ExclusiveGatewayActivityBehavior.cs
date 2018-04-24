using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     implementation of the Exclusive Gateway/XOR gateway/exclusive data-based gateway
    ///     as defined in the BPMN specification.
    ///     
    /// </summary>
    public class ExclusiveGatewayActivityBehavior : GatewayActivityBehavior
    {
        protected internal new static BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        /// <summary>
        ///     The default behaviour of BPMN, taking every outgoing sequence flow
        ///     (where the condition evaluates to true), is not valid for an exclusive
        ///     gateway.
        ///     Hence, this behaviour is overriden and replaced by the correct behavior:
        ///     selecting the first sequence flow which condition evaluates to true
        ///     (or which hasn't got a condition) and leaving the activity through that
        ///     sequence flow.
        ///     If no sequence flow is selected (ie all conditions evaluate to false),
        ///     then the default sequence flow is taken (if defined).
        /// </summary>
        public override void DoLeave(IActivityExecution execution)
        {
            Log.LeavingActivity(execution.Activity.Id);

            IPvmTransition outgoingSeqFlow = null;
            var defaultSequenceFlow = (string) execution.Activity.GetProperty("default");
            var transitionIterator = execution.Activity.OutgoingTransitions.GetEnumerator();
            while ((outgoingSeqFlow == null) && transitionIterator.MoveNext())
            {
                var seqFlow = transitionIterator.Current;

                var condition = (ICondition) seqFlow.GetProperty(BpmnParse.PropertynameCondition);
                if (((condition == null) &&
                     (ReferenceEquals(defaultSequenceFlow, null) || !defaultSequenceFlow.Equals(seqFlow.Id))) ||
                    ((condition != null) && condition.Evaluate(execution)))
                {
                    Log.OutgoingSequenceFlowSelected(seqFlow.Id);
                    outgoingSeqFlow = seqFlow;
                }
            }

            if (outgoingSeqFlow != null)
            {
                execution.LeaveActivityViaTransition(outgoingSeqFlow);
            }
            else
            {
                if (!ReferenceEquals(defaultSequenceFlow, null))
                {
                    var defaultTransition = execution.Activity.FindOutgoingTransition(defaultSequenceFlow);
                    if (defaultTransition != null)
                        execution.LeaveActivityViaTransition(defaultTransition);
                    else
                        throw Log.MissingDefaultFlowException(execution.Activity.Id, defaultSequenceFlow);
                }
                else
                {
                    //No sequence flow could be found, not even a default one
                    throw Log.StuckExecutionException(execution.Activity.Id);
                }
            }
        }
    }
}