using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IOrganizationUnit : IBusinessContextElement
	{

	  ICollection<IDecision> DecisionsMade {get;}

	  ICollection<IDecision> DecisionsOwned {get;}

	}

}