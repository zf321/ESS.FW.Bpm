
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Authorization.Service
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class AssignTaskListener : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.Assignee = "demo";
	  }

	}

}