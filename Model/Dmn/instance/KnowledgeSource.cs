using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IKnowledgeSource : IDrgElement
	{

	  string LocationUri {get;set;}


	  ICollection<IAuthorityRequirement> AuthorityRequirement {get;}

	  IType Type {get;set;}


	  IOrganizationUnit Owner {get;set;}


	}

}