using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN outputSet element
	/// 
	/// 
	/// </summary>
	public interface IOutputSet : IBaseElement
	{

	  string Name {get;set;}


	  ICollection<IDataOutput> DataOutputRefs {get;}

	  ICollection<IDataOutput> OptionalOutputRefs {get;}

	  ICollection<IDataOutput> WhileExecutingOutputRefs {get;}

	  ICollection<INputSet> InputSetRefs {get;}
	}

}