

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN conversationLink element
	/// 
	/// 
	/// </summary>
	public interface IConversationLink : IBaseElement
	{

	  string Name {get;set;}


	  IInteractionNode Source {get;set;}


	  IInteractionNode Target {get;set;}


	}

}