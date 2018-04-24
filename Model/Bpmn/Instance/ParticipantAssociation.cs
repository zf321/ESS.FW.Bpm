

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN participantAssociation element
	/// 
	/// 
	/// </summary>
	public interface IParticipantAssociation : IBaseElement
	{

	  IParticipant InnerParticipant {get;set;}


	  IParticipant OuterParticipant {get;set;}


	}

}