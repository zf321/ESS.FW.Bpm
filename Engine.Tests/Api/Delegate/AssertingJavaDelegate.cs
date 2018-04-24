using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Delegate
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class AssertingJavaDelegate : IJavaDelegate
	{

	  public static IList<DelegateExecutionAsserter> Asserts = new List<DelegateExecutionAsserter>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		foreach (DelegateExecutionAsserter a in Asserts)
		{
		  a.DoAssert((IDelegateExecution) execution);
		}
	  }

	  public interface DelegateExecutionAsserter
	  {
		void DoAssert(IDelegateExecution execution);
	  }

	  public static void Clear()
	  {
		Asserts.Clear();
	  }

	  public static void AddAsserts(params DelegateExecutionAsserter[] @as)
	  {
		((List<DelegateExecutionAsserter>)Asserts).AddRange((@as));
	  }

	}

}