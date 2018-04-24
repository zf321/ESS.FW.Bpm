using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime
{
    public class FailingDelegate : IJavaDelegate
    {
        public const string EXCEPTION_MESSAGE = "Expected_exception.";
        
        public void Execute(IBaseDelegateExecution execution)
        {
            var fail = (bool?) execution.GetVariable("fail");

            if (fail == null || fail == true)
                throw new ProcessEngineException(EXCEPTION_MESSAGE);
        }
    }
}