using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN inputSet element
	/// 
	/// 
	/// </summary>
	public interface INputSet : IBaseElement
	{

	  string Name {get;set;}


	  ICollection<IDataInput> DataInputs {get;}

	  ICollection<IDataInput> OptionalInputs {get;}

	  ICollection<IDataInput> WhileExecutingInput {get;}

	  ICollection<IOutputSet> OutputSets {get;}

	}

}