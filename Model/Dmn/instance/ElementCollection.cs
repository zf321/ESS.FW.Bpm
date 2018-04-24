using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IElementCollection : INamedElement
	{

	  ICollection<IDrgElement> DrgElements {get;}

	}

}