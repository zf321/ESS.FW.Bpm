
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Mgmt
{

    public class AlwaysFailingDelegate : IJavaDelegate
    {

        public const string Message = "Expected exception";
        
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new ProcessEngineException(Message);
        }

    }

}