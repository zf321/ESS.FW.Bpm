using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.MultiTenancy.Listener
{
    
	public class AssertingCaseExecutionListener : ICaseExecutionListener
	{

	  public static IList<DelegateCaseExecutionAsserter> Asserts = new List<DelegateCaseExecutionAsserter>();


	  public void Notify(IDelegateCaseExecution caseExecution)
	  {
		foreach (DelegateCaseExecutionAsserter Asserter in Asserts)
		{
		  Asserter.doAssert(caseExecution);
		}
	  }

	  public interface DelegateCaseExecutionAsserter
	  {
		void doAssert(IDelegateCaseExecution caseExecution);
	  }

	  public static void clear()
	  {
		Asserts.Clear();
	  }

	  public static void addAsserts(params DelegateCaseExecutionAsserter[] Asserters)
	  {
		((List<DelegateCaseExecutionAsserter>)Asserts).AddRange((Asserters));
	  }

	}

}