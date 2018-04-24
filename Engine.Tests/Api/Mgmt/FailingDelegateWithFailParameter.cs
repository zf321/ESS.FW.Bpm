using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Mgmt
{

	public class FailingDelegateWithFailParameter : IJavaDelegate
	{
        
	  public void Execute(IBaseDelegateExecution execution)
	  {

		bool? fail = (bool?) execution.GetVariable("fail");

		if (fail != null && (bool) fail)
		{
		  throw new ProcessEngineException("Exception expected.");
		}
	  }

	}

}