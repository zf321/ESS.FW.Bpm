

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IInputClause : IDmnElement
	{

	  IInputExpression InputExpression {get;set;}


	  INputValues InputValues {get;set;}


	  // camunda extensions

	  string CamundaInputVariable {get;set;}


	}

}