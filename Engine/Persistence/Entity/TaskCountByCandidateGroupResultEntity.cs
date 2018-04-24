using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// .
	/// </summary>
	public class TaskCountByCandidateGroupResultEntity : ITaskCountByCandidateGroupResult
	{
	  public virtual int TaskCount { get; set; }

	  public virtual string GroupName { get; set; }



	  public override string ToString()
	  {
		return this.GetType().Name + "[taskCount=" + TaskCount + ", groupName='" + GroupName + ']';
	  }
	}

}