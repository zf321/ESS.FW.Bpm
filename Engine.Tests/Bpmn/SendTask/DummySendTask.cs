

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.SendTask
{



	/// <summary>
	/// @author Kristin Polenz
	/// </summary>
	public class DummySendTask : IJavaDelegate
	{

	  public static bool WasExecuted = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		WasExecuted = true;
	  }

	}

}