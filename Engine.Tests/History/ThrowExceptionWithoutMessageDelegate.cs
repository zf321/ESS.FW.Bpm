
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ThrowExceptionWithoutMessageDelegate : IJavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		throw new ProcessEngineException();
	  }

	}

}