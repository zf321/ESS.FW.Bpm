using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface List : IExpression
	{

	  ICollection<IExpression> Expressions {get;}

	}

}