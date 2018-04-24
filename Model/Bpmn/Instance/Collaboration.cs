using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN collaboration element
	/// 
	/// 
	/// </summary>
	public interface ICollaboration : IRootElement
	{

	  string Name {get;set;}


	  bool Closed {get;set;}


	  ICollection<IParticipant> Participants {get;}

	  ICollection<IMessageFlow> MessageFlows {get;}

	  ICollection<IArtifact> Artifacts {get;}

	  ICollection<IConversationNode> ConversationNodes {get;}

	  ICollection<IConversationAssociation> ConversationAssociations {get;}

	  ICollection<IParticipantAssociation> ParticipantAssociations {get;}

	  ICollection<IMessageFlowAssociation> MessageFlowAssociations {get;}

	  ICollection<ICorrelationKey> CorrelationKeys {get;}

	  /// <summary>
	  /// TODO: choreographyRef </summary>

	  ICollection<IConversationLink> ConversationLinks {get;}

	}

}