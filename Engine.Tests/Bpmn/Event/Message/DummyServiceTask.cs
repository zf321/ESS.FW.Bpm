using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Message
{
    public class DummyServiceTask : IJavaDelegate
    {

        public static bool wasExecuted = false;

        public void Execute(IBaseDelegateExecution execution)
        {
            wasExecuted = true;
        }
    }

}