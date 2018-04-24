
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
    
	/// <summary>
	/// 
	/// </summary>
	public class CandidateGroupAssignment : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.AddCandidateGroup("management");
	  }

	}

}