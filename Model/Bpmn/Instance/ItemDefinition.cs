

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN itemDefinition element
	/// 
	/// 
	/// </summary>
	public interface IItemDefinition : IRootElement
	{

	  string StructureRef {get;set;}


	  bool Collection {get;set;}


	  ItemKind ItemKind {get;set;}

	}

}