using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class ThrowExceptionWithOverlongMessageDelegate : IJavaDelegate
	{

	  private const long serialVersionUID = 1L;
	  protected internal string exceptionMessage;

	  public ThrowExceptionWithOverlongMessageDelegate(string exceptionMessage)
	  {
		this.exceptionMessage = exceptionMessage;
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {

		throw new System.Exception(exceptionMessage);
	  }
	}

}