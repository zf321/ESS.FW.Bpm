using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN callConversation element
	/// 
	/// 
	/// </summary>
	public interface ICallConversation : IConversationNode
	{

	  IGlobalConversation CalledCollaboration {get;set;}


	  ICollection<IParticipantAssociation> ParticipantAssociations {get;}

	}

}