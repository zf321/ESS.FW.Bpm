

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN dataObjectReference element
	/// 
	/// @author Dario Campagna
	/// </summary>
	public interface IDataObjectReference : IFlowElement, IItemAwareElement
	{

	  IDataObject DataObject {get;set;}


	}

}