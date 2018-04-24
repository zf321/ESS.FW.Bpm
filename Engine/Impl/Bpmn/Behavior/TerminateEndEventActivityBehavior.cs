using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     <para>The BPMN terminate End Event.</para>
    ///     <para>
    ///         The start behavior of the terminate end event is <seealso cref="ActivityStartBehavior#INTERRUPT_FLOW_SCOPE" />.
    ///         as a result, the current scope will be interrupted (all concurrent executions cancelled) and this
    ///         behavior is entered with the scope execution.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public class TerminateEndEventActivityBehavior : FlowNodeActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
            // we are the last execution inside this scope: calling end() ends this scope.
            execution.End(true);
        }
    }
}