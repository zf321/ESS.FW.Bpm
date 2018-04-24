using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{

    /// <summary>
    ///     Implementation of the BPMN 2.0 subprocess (formally known as 'embedded' subprocess):
    ///     a subprocess defined within another process definition.
    ///     事务？
    /// </summary>
    public class SubProcessActivityBehavior : AbstractBpmnActivityBehavior, ICompositeActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
            IPvmActivity initialActivity = execution.Activity.Properties.Get(BpmnProperties.InitialActivity);

            EnsureUtil.EnsureNotNull("No initial activity found for subprocess " + execution.Activity.Id,
                "initialActivity", initialActivity);

            execution.ExecuteActivity(initialActivity);
        }

        public virtual void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            // join
            endedExecution.Remove();
            scopeExecution.TryPruneLastConcurrentChild();
            scopeExecution.ForceUpdate();
        }

        public virtual void Complete(IActivityExecution scopeExecution)
        {
            Leave(scopeExecution);
        }

        public override void DoLeave(IActivityExecution execution)
        {
            CompensationUtil.CreateEventScopeExecution((ExecutionEntity) execution);

            base.DoLeave(execution);
        }
    }
}