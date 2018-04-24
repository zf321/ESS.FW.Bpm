using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables.Scope
{
    
	/// <summary>
	/// 
	/// </summary>
	public class AssertVariableScopeDelegate : IJavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public void Execute(IBaseDelegateExecution execution)
	  {
		Assert.That(execution.GetVariableLocal("targetOrderId"),Is.EqualTo(null));
	  }
	}

}