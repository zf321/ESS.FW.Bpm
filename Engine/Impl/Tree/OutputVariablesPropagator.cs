using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Pass the output variables from the process instance of a subprocess to the
    ///     calling process instance.
    ///     
    /// </summary>
    public class OutputVariablesPropagator : ITreeVisitor<IActivityExecution>
    {
        public virtual void Visit(IActivityExecution execution)
        {
            if (IsProcessInstanceOfSubprocess(execution))
            {
                var superExecution = (PvmExecutionImpl) execution.SuperExecution;
                var activity = superExecution.Activity;
                var subProcessActivityBehavior = (ISubProcessActivityBehavior) activity.ActivityBehavior;

                subProcessActivityBehavior.PassOutputVariables(superExecution, execution);
            }
        }

        protected internal virtual bool IsProcessInstanceOfSubprocess(IActivityExecution execution)
        {
            return execution.IsProcessInstanceExecution && (execution.SuperExecution != null);
        }
    }
}