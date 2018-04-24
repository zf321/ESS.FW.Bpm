using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface INvocation : IExpression
	{

	  IExpression Expression {get;set;}


	  ICollection<IBinding> Bindings {get;}

	}

}