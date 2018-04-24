

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN itemAwareElement element
	/// 
	/// 
	/// </summary>
	public interface IItemAwareElement : IBaseElement
	{

	  IItemDefinition ItemSubject {get;set;}


	  IDataState DataState {get;set;}


	}

}