using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     Implements <seealso cref="ActivityStartBehavior#CANCEL_EVENT_SCOPE" />.
    ///      Throben Lindhauer
    ///     
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationCancelActivity : IPvmAtomicOperation
    {
        public abstract bool AsyncCapable { get; }

        public virtual string CanonicalName
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            // Assumption: execution is scope
            var cancellingActivity = execution.NextActivity;
            execution.NextActivity = null;

            // first, cancel and destroy the current scope
            execution.IsActive = true;

            PvmExecutionImpl propagatingExecution = null;

            if (LegacyBehavior.IsConcurrentScope(execution))
            {
                // this is legacy behavior
                LegacyBehavior.CancelConcurrentScope(execution, (IPvmActivity) cancellingActivity.EventScope);
                propagatingExecution = execution;
            }
            else
            {
                // Unlike PvmAtomicOperationTransitionDestroyScope this needs to use delete() (instead of destroy() and remove()).
                // The reason is that PvmAtomicOperationTransitionDestroyScope is executed when a scope (or non scope) is left using
                // a sequence flow. In that case the execution will have completed all the work inside the current activity
                // and will have no more child executions. In PvmAtomicOperationCancelScope the scope is cancelled due to
                // a boundary event firing. In that case the execution has not completed all the work in the current scope / activity
                // and it is necessary to delete the complete hierarchy of executions below and including the execution itself.
                execution.DeleteCascade("Cancel scope activity " + cancellingActivity + " executed.");
                propagatingExecution = (PvmExecutionImpl) execution.Parent;
            }

            propagatingExecution.Activity=(ActivityImpl) (cancellingActivity);
            propagatingExecution.IsActive = true;
            propagatingExecution.IsEnded = false;
            ActivityCancelled(propagatingExecution);
        }

        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        protected internal abstract void ActivityCancelled(PvmExecutionImpl execution);
    }
}