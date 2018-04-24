using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.MultiTenancy
{
    

	public class FailingDelegate : IJavaDelegate
	{
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		  throw new ProcessEngineException("Expected exception");
	  }
	}

}