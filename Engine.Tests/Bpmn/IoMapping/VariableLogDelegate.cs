using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.IoMapping
{
	/// <summary>
	/// 
	/// </summary>
	public class VariableLogDelegate : IJavaDelegate, IDelegateListener<IBaseDelegateExecution>
    {

	  public static IDictionary<string, object> LOCAL_VARIABLES = new Dictionary<string, object>();
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		LOCAL_VARIABLES = execution.VariablesLocal;
	  }

	  public static void Reset()
	  {
		LOCAL_VARIABLES = new Dictionary<string, object>();
	  }
	  public virtual void Notify(IBaseDelegateExecution execution)
	  {
		LOCAL_VARIABLES = execution.VariablesLocal;
	  }

	}

}