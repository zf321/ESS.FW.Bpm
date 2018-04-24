

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN escalation element
	/// 
	/// 
	/// </summary>
	public interface IEscalation : IRootElement
	{

	  string Name {get;set;}


	  string EscalationCode {get;set;}


	  IItemDefinition Structure {get;set;}


	}

}