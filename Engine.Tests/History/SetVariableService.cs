
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    
	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class SetVariableService : IJavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public  void Execute(IBaseDelegateExecution execution)
	  {
		execution.SetVariable("testVar", "testValue");
	  }

	}

}