

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN resourceParameter element
	/// 
	/// 
	/// </summary>
	public interface IResourceParameter : IBaseElement
	{

	  string Name {get;set;}


	  IItemDefinition Type {get;set;}


	  bool Required {get;set;}

	}

}