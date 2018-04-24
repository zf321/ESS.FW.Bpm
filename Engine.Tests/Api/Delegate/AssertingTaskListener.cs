using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Delegate
{

    

	public class AssertingTaskListener : ITaskListener
	{

	  public static IList<DelegateTaskAsserter> Asserts = new List<DelegateTaskAsserter>();

	  public void Notify(IDelegateTask delegateTask)
	  {
		foreach (DelegateTaskAsserter Asserter in Asserts)
		{
		  Asserter.doAssert(delegateTask);
		}
	  }

	  public interface DelegateTaskAsserter
	  {
		void doAssert(IDelegateTask task);
	  }

	  public static void clear()
	  {
		Asserts.Clear();
	  }

	  public static void addAsserts(params DelegateTaskAsserter[] Asserters)
	  {
		((List<DelegateTaskAsserter>)Asserts).AddRange((Asserters));
	  }

	}

}