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
	public class UpdateValueDelegate : IJavaDelegate
	{

	  private const long serialVersionUID = 1L;

	  public const string NEW_ELEMENT = "new element";

	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		IList<string> list = (IList<string>) execution.GetVariable("listVar");
		// implicit update of the list, so no execution.SetVariable call
		list.Add(NEW_ELEMENT);
	  }

	}

}