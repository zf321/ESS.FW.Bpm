using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.History
{

	/// <summary>
	/// 
	/// </summary>
	public class Noop : IActivityBehavior
	{
        
	  public virtual void Execute(IActivityExecution execution)
	  {
		IPvmTransition transition = execution.Activity.OutgoingTransitions[0];
		execution.LeaveActivityViaTransition(transition);
	  }

	}

}