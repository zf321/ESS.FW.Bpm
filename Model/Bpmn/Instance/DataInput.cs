

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN dataInput element
	/// 
	/// 
	/// </summary>
	public interface IDataInput : IItemAwareElement
	{

	  string Name {get;set;}


	  bool Collection {get;set;}


	}

}