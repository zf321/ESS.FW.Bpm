
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class FirstFailingDelegate : IJavaDelegate
	{

	  public const string FIRST_EXCEPTION_MESSAGE = "First expected exception.";
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		bool? fail = (bool?) execution.GetVariable("firstFail");

		if (fail == null || fail == true)
		{
		  throw new ProcessEngineException(FIRST_EXCEPTION_MESSAGE);
		}
	  }

	}

}