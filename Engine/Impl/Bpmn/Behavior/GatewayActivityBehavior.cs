using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     super class for all gateway activity implementations.
    ///     
    /// </summary>
    public abstract class GatewayActivityBehavior : FlowNodeActivityBehavior
    {
        protected internal virtual void LockConcurrentRoot(IActivityExecution execution)
        {
            IActivityExecution concurrentRoot = null;
            if (execution.IsConcurrent)
                concurrentRoot = execution.Parent;
            else
                concurrentRoot = execution;
            ((ExecutionEntity)concurrentRoot).ForceUpdate();
        }
    }
}