using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.History
{

    

	/// <summary>
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class UpdateAndReplaceValueDelegate : IJavaDelegate
	{

	  private const long serialVersionUID = 1L;

	  public const string NEW_ELEMENT = "new element";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		IList<string> list = (IList<string>) execution.GetVariable("listVar");

		// implicitly update the previous list, should update the variable value
		list.Add(NEW_ELEMENT);

		// replace the list by another object
		execution.SetVariable("listVar", new List<string>());

		// note that this is the condensed form of more realistic scenarios like
		// an implicit update in task 1 and an explicit update in the following task 2,
		// both in the same transaction.
	  }

	}

}