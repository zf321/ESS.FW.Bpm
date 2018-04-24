using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IBusinessKnowledgeModel : IDrgElement
	{

	  IEncapsulatedLogic EncapsulatedLogic {get;set;}


	  IVariable Variable {get;set;}


	  ICollection<IKnowledgeRequirement> KnowledgeRequirement {get;}

	  ICollection<IAuthorityRequirement> AuthorityRequirement {get;}

	}

}