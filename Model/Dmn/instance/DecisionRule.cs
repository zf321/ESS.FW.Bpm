using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IDecisionRule : IDmnElement
	{

	  ICollection<IInputEntry> InputEntries {get;}

	  ICollection<IOutputEntry> OutputEntries {get;}

	}

}