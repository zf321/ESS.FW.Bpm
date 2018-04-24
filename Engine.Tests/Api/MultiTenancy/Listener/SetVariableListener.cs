
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.MultiTenancy.Listener
{
    

	public class SetVariableListener : ICaseExecutionListener
	{
        
	  public void Notify(IDelegateCaseExecution caseExecution)
	  {
		caseExecution.SetVariable("var", "test");
	  }

	}

}