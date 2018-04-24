using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationActivityInstanceEnd : AbstractPvmEventAtomicOperation
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        protected internal override PvmExecutionImpl EventNotificationsStarted(PvmExecutionImpl execution)
        {
            execution.IncrementSequenceCounter();

            // hack around execution tree structure not being in sync with activity instance concept:
            // if we end a scope activity, take remembered activity instance from parent and set on
            // execution before calling END listeners.
            var parent = execution.Parent;
            IPvmActivity activity = execution.Activity;
            if ((parent != null) && execution.IsScope && (activity != null) && activity.IsScope &&
                (activity.ActivityBehavior is ICompositeActivityBehavior ||
                 (CompensationBehavior.IsCompensationThrowing(execution) &&
                  !LegacyBehavior.IsCompensationThrowing(execution))))
            {
                Log.DebugLeavesActivityInstance(execution, execution.ActivityInstanceId);

                // use remembered activity instance id from parent
                execution.ActivityInstanceId = parent.ActivityInstanceId;
                // make parent go one scope up.
                parent.LeaveActivityInstance();
            }

            return execution;
        }

        protected internal override bool IsSkipNotifyListeners(PvmExecutionImpl execution)
        {
            // listeners are skipped if this execution is not part of an activity instance.
            return ReferenceEquals(execution.ActivityInstanceId, null);
        }
    }
}