using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class PvmAtomicOperationDeleteCascadeFireActivityEnd : PvmAtomicOperationActivityInstanceEnd
    {
        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameEnd; }
        }

        public override string CanonicalName
        {
            get { return "delete-cascade-fire-activity-end"; }
        }

        protected internal override PvmExecutionImpl EventNotificationsStarted(PvmExecutionImpl execution)
        {
            execution.Canceled = true;
            return base.EventNotificationsStarted(execution);
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            var activity = execution.Activity;

            if (activity != null)
                return activity as CoreModelElement;
            // TODO: when can this happen?
            var parent = execution.Parent;
            if (parent != null)
                return GetScope((PvmExecutionImpl) execution.Parent);
            return execution.ProcessDefinition;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            IPvmActivity activity = execution.Activity;

            if (execution.IsScope && (ExecutesNonScopeActivity(execution) || IsAsyncBeforeActivity(execution)) &&
                !CompensationBehavior.executesNonScopeCompensationHandler(execution))
            {
                // case this is a scope execution and the activity is not a scope
                execution.LeaveActivityInstance();
                execution.Activity=(GetFlowScopeActivity(activity));
                execution.PerformOperation(PvmAtomicOperationFields.DeleteCascadeFireActivityEnd);
            }
            else
            {
                if (execution.IsScope)
                    execution.Destroy();

                // remove this execution and its concurrent parent (if exists)
                execution.Remove();

                var continueRemoval = !execution.IsDeleteRoot;

                if (continueRemoval)
                {
                    var propagatingExecution = execution.Parent;
                    if ((propagatingExecution != null) && !propagatingExecution.IsScope &&
                        !propagatingExecution.HasChildren())
                    {
                        propagatingExecution.Remove();
                        continueRemoval = !propagatingExecution.IsDeleteRoot;
                        propagatingExecution = propagatingExecution.Parent;
                    }

                    if (continueRemoval)
                        if (propagatingExecution != null)
                        {
                            // continue deletion with the next scope execution
                            // set activity on parent in case the parent is an inactive scope execution and activity has been set to 'null'.
                            if ((propagatingExecution.Activity == null) && (activity != null) &&
                                (activity.FlowScope != null))
                                propagatingExecution.Activity=(GetFlowScopeActivity(activity));
                            ((PvmExecutionImpl)propagatingExecution).PerformOperation(PvmAtomicOperationFields.DeleteCascade);
                        }
                }
            }
        }

        protected internal virtual bool ExecutesNonScopeActivity(PvmExecutionImpl execution)
        {
            var activity = execution.Activity;
            return (activity != null) && !activity.IsScope;
        }

        protected internal virtual bool IsAsyncBeforeActivity(PvmExecutionImpl execution)
        {
            return !ReferenceEquals(execution.ActivityId, null) && ReferenceEquals(execution.ActivityInstanceId, null);
        }

        protected internal virtual ActivityImpl GetFlowScopeActivity(IPvmActivity activity)
        {
            var flowScope = activity.FlowScope;
            ActivityImpl flowScopeActivity = null;
            if (flowScope.ProcessDefinition != flowScope)
                flowScopeActivity = (ActivityImpl) flowScope;
            return flowScopeActivity;
        }
    }
}