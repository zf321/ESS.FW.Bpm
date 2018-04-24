

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN dataOutput element
	/// 
	/// 
	/// </summary>
	public interface IDataOutput : IItemAwareElement
	{

	  string Name {get;set;}


	  bool Collection {get;set;}


	}

}