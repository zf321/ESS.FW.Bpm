using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;

namespace Engine.Tests.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class UpdateHistoricDetailValueDelegate : IJavaDelegate
	{

	  private const long serialVersionUID = 1L;

	  public const string NEW_ELEMENT = "new element";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
	  public virtual void Execute(IBaseDelegateExecution execution)
	  {
		IHistoryService historyService = ((IDelegateExecution)execution).ProcessEngineServices.HistoryService;

		IHistoricVariableInstance variableInstance = historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("listVar")*/.First();

		IHistoricVariableUpdate initialUpdate = (IHistoricVariableUpdate) historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(variableInstance.Id)//.OrderPartiallyByOccurrence()*//*.Asc()*/.First();

		IList<string> list = (IList<string>) initialUpdate.Value;

		// implicit update of the list, should not trigger an update
		// of the value since we deal with historic variables
		list.Add(NEW_ELEMENT);
	  }

	}

}