using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractInstanceCancellationCmd : AbstractProcessInstanceModificationCommand
    {
        public AbstractInstanceCancellationCmd(string processInstanceId) : base(processInstanceId)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            var sourceInstanceExecution = DetermineSourceInstanceExecution(commandContext);

            // Outline:
            // 1. find topmost scope execution beginning at scopeExecution that has exactly
            //    one child (this is the topmost scope we can cancel)
            // 2. cancel all children of the topmost execution
            // 3. cancel the activity of the topmost execution itself (if applicable)
            // 4. remove topmost execution (and concurrent parent) if topmostExecution is not the process instance

            var topmostCancellableExecution = sourceInstanceExecution;
            ExecutionEntity parentScopeExecution = (ExecutionEntity) topmostCancellableExecution.GetParentScopeExecution(false);

            // if topmostCancellableExecution's scope execution has no other non-event-scope children,
            // we have reached the correct execution
            while (parentScopeExecution != null && (parentScopeExecution.NonEventScopeExecutions.Count <= 1))
            {
                topmostCancellableExecution = parentScopeExecution;
                parentScopeExecution = (ExecutionEntity)topmostCancellableExecution.GetParentScopeExecution(false);
            }

            if (topmostCancellableExecution.PreserveScope)
            {
                topmostCancellableExecution.Interrupt("Cancelled due to process instance modification", skipCustomListeners, skipIoMappings);
                topmostCancellableExecution.LeaveActivityInstance();
                topmostCancellableExecution.Activity=(null);
            }
            else
            {
                topmostCancellableExecution.DeleteCascade("Cancelled due to process instance modification", skipCustomListeners, skipIoMappings);
                HandleChildRemovalInScope(topmostCancellableExecution);

            }

            return null;
        }

        protected internal virtual void HandleChildRemovalInScope(ExecutionEntity removedExecution)
        {
            // TODO: the following should be closer to PvmAtomicOperationDeleteCascadeFireActivityEnd
            // (note though that e.g. boundary events expect concurrent executions to be preserved)
            //
            // Idea: attempting to prune and synchronize on the parent is the default behavior when
            // a concurrent child is removed, but scope activities implementing ModificationObserverBehavior
            // override this default (and thereforemust* take care of reorganization themselves)

            // notify the behavior that a concurrent execution has been removed

            // must be set due to deleteCascade behavior
            ActivityImpl activity = removedExecution.Activity as ActivityImpl;
            ScopeImpl flowScope = activity.FlowScope;

            IActivityExecution scopeExecution = removedExecution.GetParentScopeExecution(false);
            IActivityExecution executionInParentScope = removedExecution.IsConcurrent ? removedExecution : removedExecution.Parent;

            if (flowScope.ActivityBehavior != null && flowScope.ActivityBehavior is IModificationObserverBehavior)
            {
                // let child removal be handled by the scope itself
                IModificationObserverBehavior behavior = (IModificationObserverBehavior)flowScope.ActivityBehavior;
                behavior.DestroyInnerInstance(executionInParentScope);
            }
            else
            {
                if (executionInParentScope.IsConcurrent)
                {
                    executionInParentScope.Remove();
                    scopeExecution.TryPruneLastConcurrentChild();
                    scopeExecution.ForceUpdate();
                }
            }
        }

        protected internal abstract ExecutionEntity DetermineSourceInstanceExecution(CommandContext commandContext);
    }
}