

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IBinding : IDmnModelElementInstance
	{

	  INformationItem Parameter {get;set;}


	  IExpression Expression {get;set;}


	}

}