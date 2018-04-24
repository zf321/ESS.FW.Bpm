using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Tuple of a scope and an execution.
    ///     
    /// </summary>
    public class ActivityExecutionTuple
    {
        public ActivityExecutionTuple(IPvmScope scope, IActivityExecution execution)
        {
            Execution = execution;
            Scope = scope;
        }

        public virtual IActivityExecution Execution { get; }

        public virtual IPvmScope Scope { get; }
    }
}