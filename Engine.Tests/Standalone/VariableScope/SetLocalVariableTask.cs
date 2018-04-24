using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Standalone.VariableScope
{
    
	public class SetLocalVariableTask : IJavaDelegate
	{
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		execution.SetVariableLocal("test", "test2");
	  }

	}

}