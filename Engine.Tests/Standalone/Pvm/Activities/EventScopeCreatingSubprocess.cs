using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class EventScopeCreatingSubprocess : ICompositeActivityBehavior
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

        /// Incoming execution is transformed into an event Scope,
        /// new, non-concurrent execution leaves activity
        public void Complete(IActivityExecution execution)
        {
            var outgoingExecution = execution.Parent.CreateExecution();
            outgoingExecution.IsConcurrent = false;
            outgoingExecution.Activity = execution.Activity;

            // eventscope execution
            execution.IsConcurrent = false;
            execution.IsActive = false;
            ((PvmExecutionImpl) execution).IsEventScope = true;

            var outgoingTransitions = execution.Activity.OutgoingTransitions;
            if (!outgoingTransitions.Any())
                outgoingExecution.End(true);
            else
                outgoingExecution.LeaveActivityViaTransitions(outgoingTransitions, new List<IActivityExecution>());
        }
    }
}