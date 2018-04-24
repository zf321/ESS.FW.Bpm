
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class SecondFailingDelegate : IJavaDelegate
	{

	  public const string SECOND_EXCEPTION_MESSAGE = "Second expected exception.";
        
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		bool? fail = (bool?) execution.GetVariable("secondFail");

		if (fail == null || fail == true)
		{
		  throw new ProcessEngineException(SECOND_EXCEPTION_MESSAGE);
		}
	  }

	}

}