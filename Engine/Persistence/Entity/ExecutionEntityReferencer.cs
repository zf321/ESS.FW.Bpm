


using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


	/// <summary>
	/// 
	/// </summary>
	public class ExecutionEntityReferencer : IVariableStoreObserver
	{

	  protected internal ExecutionEntity execution;

	  public ExecutionEntityReferencer(ExecutionEntity _execution)
	  {
		this.execution = _execution;
	  }

	  public  void OnAdd(ICoreVariableInstance variable)
	  {
            variable.Execution= execution;
        }

	  public  void OnRemove(ICoreVariableInstance variable)
	  {
            variable.Execution = null;
        }

	}

}