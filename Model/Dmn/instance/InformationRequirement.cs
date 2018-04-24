

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface INformationRequirement : IDmnModelElementInstance
	{

	  IDecision RequiredDecision {get;set;}


	  INputData RequiredInput {get;set;}


	}

}