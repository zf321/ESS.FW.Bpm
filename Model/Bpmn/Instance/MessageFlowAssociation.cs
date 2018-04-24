

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN messageFlowAssociation element
	/// 
	/// 
	/// </summary>
	public interface IMessageFlowAssociation : IBaseElement
	{

	  IMessageFlow InnerMessageFlow {get;set;}


	  IMessageFlow OuterMessageFlow {get;set;}


	}

}