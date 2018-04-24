using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractIntermediateThrowEventBuilder : AbstractThrowEventBuilder<INtermediateThrowEvent> 
	{

	  protected internal AbstractIntermediateThrowEventBuilder(IBpmnModelInstance modelInstance, INtermediateThrowEvent element) : base(modelInstance, element)
	  {
	  }
	}

}