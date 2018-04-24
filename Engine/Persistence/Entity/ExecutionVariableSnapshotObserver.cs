
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


	/// <summary>
	/// Provides access to the snapshot of latest variables of an execution.
	/// 
	///  
	/// </summary>
	public class ExecutionVariableSnapshotObserver : IExecutionObserver
	{

	  /// <summary>
	  /// The variables which are observed during the execution.
	  /// </summary>
	  protected internal IVariableMap VariableSnapshot;

	  protected internal IPvmProcessInstance Execution;

	  public ExecutionVariableSnapshotObserver(IPvmProcessInstance executionEntity)
	  {
		this.Execution = executionEntity;
		this.Execution.AddExecutionObserver(this);
	  }

	  public  void OnClear(ExecutionEntity execution)
	  {
		if (VariableSnapshot == null)
		{
		  VariableSnapshot = execution.GetVariablesLocalTyped(false);
		}
	  }

	  public virtual IVariableMap Variables
	  {
		  get
		  {
			if (VariableSnapshot == null)
			{
			  return Execution.GetVariablesLocalTyped(false);
			}
			else
			{
			  return VariableSnapshot;
			}
		  }
	  }
	}

}