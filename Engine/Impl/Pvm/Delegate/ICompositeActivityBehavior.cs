using ESS.FW.Bpm.Engine.Impl.Core.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{
    /// <summary>
    ///      
    /// </summary>
    public interface ICompositeActivityBehavior : IActivityBehavior
    {
        /// <summary>
        ///     Invoked when an execution is ended within the scope of the composite
        /// </summary>
        /// <param name="scopeExecution"> scope execution for the activity which defined the behavior </param>
        /// <param name="endedExecution"> the execution which ended </param>
        void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution, IActivityExecution endedExecution);

        void Complete(IActivityExecution scopeExecution);
    }
}