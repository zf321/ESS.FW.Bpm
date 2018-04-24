using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implementation of the Parallel Gateway/AND gateway as definined in the BPMN
    ///     2.0 specification.
    ///     The Parallel Gateway can be used for splitting a path of execution into
    ///     multiple paths of executions (AND-split/fork behavior), one for every
    ///     outgoing sequence flow.
    ///     The Parallel Gateway can also be used for merging or joining paths of
    ///     execution (AND-join). In this case, on every incoming sequence flow an
    ///     execution needs to arrive, before leaving the Parallel Gateway (and
    ///     potentially then doing the fork behavior in case of multiple outgoing
    ///     sequence flow).
    ///     Note that there is a slight difference to spec (p. 436): "The parallel
    ///     gateway is activated if there is at least one Token on each incoming sequence
    ///     flow." We only check the number of incoming tokens to the number of sequenceflow.
    ///     So if two tokens would arrive through the same sequence flow, our implementation
    ///     would activate the gateway.
    ///     Note that a Parallel Gateway having one incoming and multiple ougoing
    ///     sequence flow, is the same as having multiple outgoing sequence flow on a
    ///     given activity. However, a parallel gateway does NOT check conditions on the
    ///     outgoing sequence flow.
    ///     
    ///      
    /// </summary>
    public class ParallelGatewayActivityBehavior : GatewayActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        
        public override void Execute(IActivityExecution execution)
        {
            // Join
            var activity = execution.Activity;
            var outgoingTransitions = execution.Activity.OutgoingTransitions;

            execution.InActivate();
            LockConcurrentRoot(execution);

            var joinedExecutions = execution.FindInactiveConcurrentExecutions(activity);
            var nbrOfExecutionsToJoin = execution.Activity.IncomingTransitions.Count;
            var nbrOfExecutionsJoined = joinedExecutions.Count;

            if (nbrOfExecutionsJoined == nbrOfExecutionsToJoin)
            {
                // Fork
                Log.ActivityActivation(activity.Id, nbrOfExecutionsJoined, nbrOfExecutionsToJoin);
                execution.LeaveActivityViaTransitions(outgoingTransitions, joinedExecutions);
            }
            else
            {
                Log.NoActivityActivation(activity.Id, nbrOfExecutionsJoined, nbrOfExecutionsToJoin);
            }
        }
    }
}