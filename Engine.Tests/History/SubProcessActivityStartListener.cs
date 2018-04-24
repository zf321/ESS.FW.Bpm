using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
	/// <summary>
	///
	/// </summary>
	public class SubProcessActivityStartListener : IDelegateListener<IBaseDelegateExecution>
    {
	  public virtual void Notify(IBaseDelegateExecution execution)
	  {
		int? counter = (int?) execution.GetVariable("executionListenerCounter");
		if (counter == null)
		{
		  counter = 0;
		}
		execution.SetVariable("subExecutionListenerCounter", ++counter);
	  }
	}
}