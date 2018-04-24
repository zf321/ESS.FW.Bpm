using System.Collections.Generic;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN conditionalEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface IConditionalEventDefinition : IEventDefinition
	{

	  ICondition Condition {get;set;}


	  string CamundaVariableName {get;set;}


	  string CamundaVariableEvents {get;set;}


	  IList<string> CamundaVariableEventsList {get;set;}


	}

}