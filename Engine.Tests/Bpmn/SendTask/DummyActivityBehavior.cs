using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.SendTask
{

    public class DummyActivityBehavior : TaskActivityBehavior
    {

        public static bool WasExecuted = false;

        public static string CurrentActivityId = null;
        public static string CurrentActivityName = null;

        public virtual void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            CurrentActivityName = execution.CurrentActivityName;
            CurrentActivityId = execution.CurrentActivityId;
            Leave(execution);
        }

        protected override void PerformExecution(IActivityExecution execution)
        {
            WasExecuted = true;
        }

    }

}