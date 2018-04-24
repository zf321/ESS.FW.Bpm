

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN escalationEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface IEscalationEventDefinition : IEventDefinition
	{

	  IEscalation Escalation {get;set;}


	}

}