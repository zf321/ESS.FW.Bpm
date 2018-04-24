
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class SetAssigneeListener : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		string assignee = (string) delegateTask.GetVariable("assignee");
		delegateTask.Assignee = assignee;
	  }

	}

}