using System;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
	/// The BPMN task element
	/// </summary>
	public interface ITask : IActivity
	{

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}


	  //IBpmnShape DiagramElement {get;}

	}

}