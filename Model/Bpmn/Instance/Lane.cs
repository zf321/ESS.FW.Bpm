using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.impl.instance;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ChildLaneSet = ChildLaneSet;
	using PartitionElement = PartitionElement;

	/// <summary>
	/// The BPMN lane element
	/// 
	/// 
	/// </summary>
	public interface ILane : IBaseElement
	{

	  string Name {get;set;}


	  PartitionElement PartitionElement {get;set;}


	  PartitionElement PartitionElementChild {get;set;}


	  ICollection<IFlowNode> FlowNodeRefs {get;}

	  ChildLaneSet ChildLaneSet {get;set;}

	}

}