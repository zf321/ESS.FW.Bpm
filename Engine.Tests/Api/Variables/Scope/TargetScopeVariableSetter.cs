using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Variables.Scope
{

	/// <summary>
	/// 
	/// </summary>
	public class TargetScopeVariableSetter : IJavaDelegate
	{

      public  void Execute(IBaseDelegateExecution execution)
      {
          execution.SetVariable("targetOrderId", execution.GetVariableLocal("targetOrderId")); //,"SubProcess_1");
      }
	}

}