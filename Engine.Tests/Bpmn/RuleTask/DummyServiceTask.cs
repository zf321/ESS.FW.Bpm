

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.RuleTask
{
    
	/// <summary>
	/// 
	/// </summary>
	public class DummyServiceTask : IJavaDelegate
	{

	  public static bool WasExecuted = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		WasExecuted = true;
	  }

	}

}