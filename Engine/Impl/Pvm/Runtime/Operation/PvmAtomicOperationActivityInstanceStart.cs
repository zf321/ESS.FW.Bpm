using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     <para>
    ///         Base Atomic operation for implementing atomic operations which mark the creation
    ///         of a new activity instance.
    ///     </para>
    ///     <para>
    ///         The new activity instance is createdbefore* the START listeners are invoked
    ///         on the execution.
    ///     </para>
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationActivityInstanceStart : AbstractPvmEventAtomicOperation
    {
        protected internal override PvmExecutionImpl EventNotificationsStarted(PvmExecutionImpl execution)
        {
            execution.IncrementSequenceCounter();
            execution.ActivityInstanceStarting();
            execution.EnterActivityInstance();

            return execution;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            // hack around execution tree structure not being in sync with activity instance concept:
            // if we start a scope activity, remember current activity instance in parent
            var parent = execution.Parent;
            IPvmActivity activity = execution.Activity;
            if ((parent != null) && execution.IsScope && activity.IsScope && CanHaveChildScopes(execution))
                parent.ActivityInstanceId = execution.ActivityInstanceId;
        }

        protected internal virtual bool CanHaveChildScopes(PvmExecutionImpl execution)
        {
            IPvmActivity activity = execution.Activity;
            return activity.ActivityBehavior is ICompositeActivityBehavior ||
                   CompensationBehavior.IsCompensationThrowing(execution);
        }
    }
}