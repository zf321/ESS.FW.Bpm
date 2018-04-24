using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IPerformanceIndicator : IBusinessContextElement
	{

	  ICollection<IDecision> ImpactingDecisions {get;}

	}

}