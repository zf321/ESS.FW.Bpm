

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IOutputClause : IDmnElement
	{

	  string Name {get;set;}


	  string TypeRef {get;set;}


	  IOutputValues OutputValues {get;set;}


	  IDefaultOutputEntry DefaultOutputEntry {get;set;}


	}

}