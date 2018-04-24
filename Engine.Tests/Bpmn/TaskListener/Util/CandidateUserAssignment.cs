

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
    
	/// <summary>
	/// 
	/// </summary>
	public class CandidateUserAssignment : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		delegateTask.AddCandidateUser("kermit");
		delegateTask.AddCandidateUser("fozzie");
	  }

	}

}