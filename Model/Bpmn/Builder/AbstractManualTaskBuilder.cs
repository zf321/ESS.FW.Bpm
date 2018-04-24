using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractManualTaskBuilder : AbstractTaskBuilder<IManualTask> 
	{

	  protected internal AbstractManualTaskBuilder(IBpmnModelInstance modelInstance, IManualTask element) : base(modelInstance, element)
	  {
	  }

	}

}