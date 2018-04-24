

using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
	/// The BPMN messageFlow element
	/// 
	/// 
	/// </summary>
	public interface IMessageFlow : IBaseElement
	{

	  string Name {get;set;}


	  IInteractionNode Source {get;set;}


	  IInteractionNode Target {get;set;}


	  IMessage Message {get;set;}


	  new IBpmnEdge DiagramElement {get;}

	}

}