
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History.UserOperationLog
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ExecuteRuntimeServiceOperationTaskListener : ITaskListener
	{

	  public virtual void Notify(IDelegateTask delegateTask)
	  {
		IProcessEngineServices services = delegateTask.ProcessEngineServices;
		IRuntimeService runtimeService = services.RuntimeService;
		runtimeService.SetVariable(delegateTask.ExecutionId, "taskListenerCalled", true);
	  }

	}

}