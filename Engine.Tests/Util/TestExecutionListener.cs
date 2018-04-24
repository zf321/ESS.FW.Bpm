using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Util
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class TestExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

	  public static IList<string> CollectedEvents = new List<string>();
        
	  public virtual void Notify(IBaseDelegateExecution execution)
	  {
		string counterKey = ((IDelegateExecution)execution).CurrentActivityId + "-" + execution.EventName;
		CollectedEvents.Add(counterKey);
	  }

	  public static void Reset()
	  {
		CollectedEvents.Clear();
	  }

	}

}