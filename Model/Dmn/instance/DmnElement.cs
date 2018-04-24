

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IDmnElement : IDmnModelElementInstance
	{

	  string Id {get;set;}


	  string Label {get;set;}


	  IDescription Description {get;set;}


	  IExtensionElements ExtensionElements {get;set;}


	}

}