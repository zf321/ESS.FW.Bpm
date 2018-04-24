using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IDecision : IDrgElement
	{

	  IQuestion Question {get;set;}


	  IAllowedAnswers AllowedAnswers {get;set;}


	  IVariable Variable {get;set;}


	  ICollection<INformationRequirement> InformationRequirements {get;}

	  ICollection<IKnowledgeRequirement> KnowledgeRequirements {get;}

	  ICollection<IAuthorityRequirement> AuthorityRequirements {get;}

	  ICollection<ISupportedObjectiveReference> SupportedObjectiveReferences {get;}

	  ICollection<IPerformanceIndicator> ImpactedPerformanceIndicators {get;}

	  ICollection<IOrganizationUnit> DecisionMakers {get;}

	  ICollection<IOrganizationUnit> DecisionOwners {get;}

	  ICollection<IUsingProcessReference> UsingProcessReferences {get;}

	  ICollection<IUsingTaskReference> UsingTaskReferences {get;}

	  IExpression Expression {get;set;}

      int CamundaHistoryTimeToLive { get; set; }


    }

}