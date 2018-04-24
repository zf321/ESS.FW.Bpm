

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN conversationAssociation element
	/// 
	/// 
	/// </summary>
	public interface IConversationAssociation : IBaseElement
	{

	  IConversationNode InnerConversationNode {get;set;}


	  IConversationNode OuterConversationNode {get;set;}


	}

}