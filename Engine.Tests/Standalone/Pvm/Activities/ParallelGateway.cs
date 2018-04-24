using System.Diagnostics;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class ParallelGateway : IActivityBehavior
    {
//private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.GetLogger();

        public void Execute(IActivityExecution execution)
        {
            var activity = execution.Activity;

            var outgoingTransitions = execution.Activity.OutgoingTransitions;

            execution.InActivate();

            var joinedExecutions = execution.FindInactiveConcurrentExecutions(activity);

            var nbrOfExecutionsToJoin = execution.Activity.IncomingTransitions.Count;
            var nbrOfExecutionsJoined = joinedExecutions.Count;

            if (nbrOfExecutionsJoined == nbrOfExecutionsToJoin)
            {
                Debug.WriteLine("parallel gateway '" + activity.Id + "' activates: " + nbrOfExecutionsJoined + " of " + nbrOfExecutionsToJoin + " joined");
                execution.LeaveActivityViaTransitions(outgoingTransitions, joinedExecutions);
            }
        }
    }
}