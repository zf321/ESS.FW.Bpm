using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;

namespace Engine.Tests.Bpmn.SubProcess.Util
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class GetActInstanceDelegate : IJavaDelegate
	{

	  public static IActivityInstance ActivityInstance = null;


	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
            ActivityInstance =((IDelegateExecution) execution).ProcessEngineServices.RuntimeService.GetActivityInstance(((IDelegateExecution)execution).ProcessInstanceId);
        }

        public void Execute(IDelegateExecution execution)
        {
            ActivityInstance = execution.ProcessEngineServices.RuntimeService.GetActivityInstance(execution.ProcessInstanceId);

        }
    }

}