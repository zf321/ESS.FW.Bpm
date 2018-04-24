
namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

	/// <summary>
	/// Represents an observer for the exeuction.
	/// 
	///  
	/// </summary>
	public interface IExecutionObserver
	{

	  /// <summary>
	  /// Callback which is called in the clearExecution method of the ExecutionEntity.
	  /// </summary>
	  /// <param name="execution"> the execution which is been observed </param>
	  void OnClear(ExecutionEntity execution);
	}

}