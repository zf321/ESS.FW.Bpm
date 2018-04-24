using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm
{
    public class EmbeddedSubProcess : ICompositeActivityBehavior
    {
        public void Execute(IActivityExecution execution)
        {
            var startActivities = new List<IPvmActivity>();
            foreach (var activity in  execution.Activity.Activities)
                if (!activity.IncomingTransitions.Any()) startActivities.Add(activity);

            foreach (var startActivity in  startActivities) execution.ExecuteActivity(startActivity);
        }

        public void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution, IActivityExecution endedExecution)
        {
            endedExecution.Remove();
            scopeExecution.TryPruneLastConcurrentChild();
        }

        public void Complete(IActivityExecution execution)
        {
            var outgoingTransitions = execution.Activity.OutgoingTransitions;
            if (!outgoingTransitions.Any())
                execution.End(true);
            else
                execution.LeaveActivityViaTransitions(outgoingTransitions, new List<IActivityExecution>());
        }
    }
}