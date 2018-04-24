

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN dataStore element
	/// 
	/// 
	/// </summary>
	public interface IDataStore : IRootElement, IItemAwareElement
	{

	  string Name {get;set;}


	  int? Capacity {get;set;}


	  bool Unlimited {get;set;}


	}

}