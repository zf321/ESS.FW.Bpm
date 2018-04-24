using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Repository
{
    
	public class AlwaysFailingDelegate : IJavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public void Execute(IBaseDelegateExecution execution)
	  {
		throw new ProcessEngineException("Exception_expected.");
	  }

	}

}