using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IRelation : IExpression
	{

	  ICollection<IColumn> Columns {get;}

	  ICollection<IRow> Rows {get;}

	}

}