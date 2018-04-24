using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IDecisionService : INamedElement
	{

	  ICollection<IDecision> OutputDecisions {get;}

	  ICollection<IDecision> EncapsulatedDecisions {get;}

	  ICollection<IDecision> InputDecisions {get;}

	  ICollection<INputData> InputData {get;}

	}

}