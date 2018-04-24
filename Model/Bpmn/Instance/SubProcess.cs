using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using SubProcessBuilder = SubProcessBuilder;

	/// <summary>
	/// The BPMN subProcess element
	/// 
	/// 
	/// </summary>
	public interface ISubProcess : IActivity
	{

	  SubProcessBuilder Builder();

	  bool TriggeredByEvent { get;set;}

	  ICollection<ILaneSet> LaneSets {get;}

	  ICollection<IFlowElement> FlowElements {get;}

	  ICollection<IArtifact> Artifacts {get;}

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}

	}

}