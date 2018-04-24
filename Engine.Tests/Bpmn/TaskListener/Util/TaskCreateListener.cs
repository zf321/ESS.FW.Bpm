

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
    
	/// <summary>
	/// 
	/// </summary>
	public class TaskCreateListener : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.Description = "TaskCreateListener is listening!";
	  }

	}

}