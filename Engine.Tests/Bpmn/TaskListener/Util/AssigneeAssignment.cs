

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
    
	/// <summary>
	/// 
	/// </summary>
	public class AssigneeAssignment : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.Assignee = "kermit";
	  }

	}

}