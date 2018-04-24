using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    ///     
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityEnd : IPvmAtomicOperation
    {
        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return execution.Activity.AsyncAfter;
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            // restore activity instance id
            if (execution.ActivityInstanceId== null)
                execution.ActivityInstanceId = execution.ParentActivityInstanceId;

            IPvmActivity activity = execution.Activity;
            var activityExecutionMapping = execution.CreateActivityExecutionMapping();

            var propagatingExecution = execution;

            if (execution.IsScope && activity.IsScope)
                if (!LegacyBehavior.DestroySecondNonScope(execution))
                {
                    execution.Destroy();
                    if (!execution.IsConcurrent)
                    {
                        execution.Remove();
                        propagatingExecution = (PvmExecutionImpl) execution.Parent;
                        propagatingExecution.Activity=(execution.Activity);
                    }
                }

            propagatingExecution = (PvmExecutionImpl) LegacyBehavior.DeterminePropagatingExecutionOnEnd(propagatingExecution,
                activityExecutionMapping);
            IPvmScope flowScope = activity.FlowScope;

            // 1. flow scope = Process Definition
            if (flowScope == activity.ProcessDefinition)
            {
                // 1.1 concurrent execution => end + tryPrune()
                if (propagatingExecution.IsConcurrent)
                {
                    propagatingExecution.Remove();
                    propagatingExecution.Parent.TryPruneLastConcurrentChild();
                    propagatingExecution.Parent.ForceUpdate();
                }
                else
                {
                    // 1.2 Process End
                    if (!propagatingExecution.PreserveScope)
                        propagatingExecution.PerformOperation(PvmAtomicOperationFields.ProcessEnd);
                }
            }
            else
            {
                // 2. flowScope != process definition
                var flowScopeActivity = (IPvmActivity) flowScope;

                var activityBehavior = flowScopeActivity.ActivityBehavior;
                if (activityBehavior is ICompositeActivityBehavior)
                {
                    var compositeActivityBehavior = (ICompositeActivityBehavior) activityBehavior;
                    // 2.1 Concurrent execution => composite behavior.concurrentExecutionEnded()
                    if (propagatingExecution.IsConcurrent && !LegacyBehavior.IsConcurrentScope(propagatingExecution))
                    {
                        compositeActivityBehavior.ConcurrentChildExecutionEnded(propagatingExecution.Parent,
                            propagatingExecution);
                    }
                    else
                    {
                        // 2.2 Scope Execution => composite behavior.complete()
                        propagatingExecution.Activity=(ActivityImpl) (flowScopeActivity);
                        compositeActivityBehavior.Complete(propagatingExecution);
                    }
                }
                else
                {
                    // activity behavior is not composite => this is unexpected
                    throw new ProcessEngineException("Expected behavior of composite scope " + activity +
                                                     " to be a CompositeActivityBehavior but got " + activityBehavior);
                }
            }
        }

        public virtual string CanonicalName
        {
            get { return "activity-end"; }
        }

        protected internal virtual IPvmScope GetScope(PvmExecutionImpl execution)
        {
            return execution.Activity;
        }
    }
}