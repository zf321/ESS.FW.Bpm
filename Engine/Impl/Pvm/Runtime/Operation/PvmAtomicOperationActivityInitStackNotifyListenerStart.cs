using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityInitStackNotifyListenerStart : PvmAtomicOperationActivityInstanceStart
    {
        public override string CanonicalName
        {
            get { return "activity-init-stack-notify-listener-start"; }
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

            execution.ActivityInstanceStarted();

            var startContext = execution.ExecutionStartContext;
            var instantiationStack = startContext.InstantiationStack;

            var propagatingExecution = execution;
            var activity = execution.Activity;
            if (activity.ActivityBehavior is IModificationObserverBehavior)
            {
                var behavior = (IModificationObserverBehavior) activity.ActivityBehavior;
                var concurrentExecutions = behavior.InitializeScope(propagatingExecution, 1);
                propagatingExecution = (PvmExecutionImpl) concurrentExecutions[0];
            }

            // if the stack has been instantiated
            if ((instantiationStack.Activities.Count == 0) && (instantiationStack.TargetActivity != null))
            {
                // as if we are entering the target activity instance id via a transition
                propagatingExecution.ActivityInstanceId = null;

                // execute the target activity with this execution
                startContext.ApplyVariables(propagatingExecution);
                propagatingExecution.Activity=(ActivityImpl) (instantiationStack.TargetActivity);
                propagatingExecution.PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
            }
            else if ((instantiationStack.Activities.Count == 0) && (instantiationStack.TargetTransition != null))
            {
                // as if we are entering the target activity instance id via a transition
                propagatingExecution.ActivityInstanceId = null;

                // execute the target transition with this execution
                var transition = instantiationStack.TargetTransition;
                startContext.ApplyVariables(propagatingExecution);
                propagatingExecution.Activity=(ActivityImpl) (transition.Source);
                propagatingExecution.Transition=((TransitionImpl) transition);
                propagatingExecution.PerformOperation(PvmAtomicOperationFields.TransitionStartNotifyListenerTake);
            }
            else
            {
                // else instantiate the activity stack further
                propagatingExecution.Activity=(null);
                propagatingExecution.PerformOperation(PvmAtomicOperationFields.ActivityInitStack);
            }
        }
    }
}