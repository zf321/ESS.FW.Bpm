using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN subConversation element
	/// 
	/// 
	/// </summary>
	public interface ISubConversation : IConversationNode
	{

	  ICollection<IConversationNode> ConversationNodes {get;}

	}

}