using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{
    public class AsyncService : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            AsyncTaskTest.INVOCATION = true;
            AsyncTaskTest.NUM_INVOCATIONS++;
        }
        

    }

}