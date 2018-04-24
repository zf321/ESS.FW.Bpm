using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN laneSet element
	/// 
	/// 
	/// </summary>
	public interface ILaneSet : IBaseElement
	{

	  string Name {get;set;}


	  ICollection<ILane> Lanes {get;}

	}

}