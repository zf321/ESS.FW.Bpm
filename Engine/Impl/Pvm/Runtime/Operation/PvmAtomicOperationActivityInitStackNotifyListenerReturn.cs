using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityInitStackNotifyListenerReturn : PvmAtomicOperationActivityInstanceStart
    {
        public override string CanonicalName
        {
            get { return "activity-init-stack-notify-listener-return"; }
        }

        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameStart; }
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            var activity = execution.Activity;

            if (activity != null)
                return activity as CoreModelElement;
            var parent = execution.Parent;
            if (parent != null)
                return GetScope((PvmExecutionImpl) execution.Parent);
            return execution.ProcessDefinition;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            base.EventNotificationsCompleted(execution);

            var startContext = execution.ExecutionStartContext;
            var instantiationStack = startContext.InstantiationStack;

            // if the stack has been instantiated
            if (instantiationStack.Activities.Count == 0)
            {
                // done
            }
            else
            {
                // else instantiate the activity stack further
                execution.Activity=(null);
                execution.PerformOperation(PvmAtomicOperationFields.ActivityInitStackAndReturn);
            }
        }
    }
}