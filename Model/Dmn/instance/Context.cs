using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IContext : IExpression
	{

	  ICollection<IContextEntry> ContextEntries {get;}

	}

}