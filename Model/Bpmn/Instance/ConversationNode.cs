using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN conversationNode element
	/// 
	/// 
	/// </summary>
	public interface IConversationNode : IBaseElement, IInteractionNode
	{

	  string Name {get;set;}


	  ICollection<IParticipant> Participants {get;}

	  ICollection<IMessageFlow> MessageFlows {get;}

	  ICollection<ICorrelationKey> CorrelationKeys {get;}

	}

}