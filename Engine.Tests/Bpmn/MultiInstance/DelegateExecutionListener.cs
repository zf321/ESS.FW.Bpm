
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class DelegateExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        
	  public  void Notify(IBaseDelegateExecution execution)
	  {
		DelegateEvent.RecordEventFor((IDelegateExecution) execution);
	  }
        
	}

}