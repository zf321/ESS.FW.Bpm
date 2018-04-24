

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IAuthorityRequirement : IDmnModelElementInstance
	{

	  IDecision RequiredDecision {get;set;}


	  INputData RequiredInput {get;set;}


	  IKnowledgeSource RequiredAuthority {get;set;}


	}

}