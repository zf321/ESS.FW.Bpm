




using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


	/// <summary>
	/// 
	/// </summary>
	public class TaskEntityReferencer : IVariableStoreObserver
	{

	  protected internal TaskEntity Task;

	  public TaskEntityReferencer(TaskEntity task)
	  {
		this.Task = task;
	  }

	  public  void OnAdd(ICoreVariableInstance variable)
	  {
		variable.Task=Task;
	  }

	  public  void OnRemove(ICoreVariableInstance variable)
	  {
		variable.Task=null;
	  }
	}

}