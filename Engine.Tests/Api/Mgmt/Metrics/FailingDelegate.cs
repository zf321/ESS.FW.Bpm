using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    
	public class FailingDelegate : IJavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {

		bool? fail = (bool?) execution.GetVariable("fail");

		if (fail != null && fail == true)
		{
		  throw new ProcessEngineException("Expected exception");
		}
	  }

	}

}