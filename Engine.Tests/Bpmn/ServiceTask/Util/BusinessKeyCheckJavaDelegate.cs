
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    public class BusinessKeyCheckJavaDelegate : IJavaDelegate
	{
        public void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariable("businessKeySetOnExecution", execution.BusinessKey );//IProcessBusinessKey
        }
    }

}