
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class IncrementVariableService : IJavaDelegate
	{
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		string aVariable = "aVariable";
		int? newValue = 1;

		object value = execution.GetVariable(aVariable);

		if (value != null)
		{
		  newValue = (int?) value;
		  newValue++;
		}

		execution.SetVariable(aVariable, newValue);
	  }

	}

}