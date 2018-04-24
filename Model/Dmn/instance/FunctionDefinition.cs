using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IFunctionDefinition : IExpression
	{

	  ICollection<IFormalParameter> FormalParameters {get;}

	  IExpression Expression {get;set;}


	}

}