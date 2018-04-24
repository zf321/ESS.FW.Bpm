using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
	/// <summary>
	///
	/// </summary>
	public class CompletingTaskListener : ITaskListener
	{
	  public void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.Complete();
	  }
	}

}