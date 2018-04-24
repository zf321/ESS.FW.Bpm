using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Standalone.Entity
{
	public class TestExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

	  public void Notify(IBaseDelegateExecution execution)
	  {
		// do nothing
	  }

	}

}