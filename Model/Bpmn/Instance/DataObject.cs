

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN dataObject element
	/// 
	/// @author Dario Campagna
	/// </summary>
	public interface IDataObject : IFlowElement, IItemAwareElement
	{

	  bool Collection {get;set;}


	}

}