using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{


	public interface IDecisionTable : IExpression
	{

	  HitPolicy HitPolicy {get;set;}


	  BuiltinAggregator Aggregation {get;set;}


	  DecisionTableOrientation PreferredOrientation {get;set;}


	  string OutputLabel {get;set;}


	  ICollection<IInput> Inputs {get;}

	  ICollection<IOutput> Outputs {get;}

	  ICollection<IRule> Rules {get;}

	}

}