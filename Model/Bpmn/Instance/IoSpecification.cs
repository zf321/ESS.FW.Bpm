using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN inputOutputSpecification element
	/// 
	/// 
	/// </summary>
	public interface IOSpecification : IBaseElement
	{

	  ICollection<IDataInput> DataInputs {get;}

	  ICollection<IDataOutput> DataOutputs {get;}

	  ICollection<INputSet> InputSets {get;}

	  ICollection<IOutputSet> OutputSets {get;}

	}

}