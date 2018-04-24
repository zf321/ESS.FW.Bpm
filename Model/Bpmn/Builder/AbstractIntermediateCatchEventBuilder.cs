using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractIntermediateCatchEventBuilder<TB> : AbstractCatchEventBuilder<INtermediateCatchEvent> where TB : AbstractIntermediateCatchEventBuilder<TB>
	{

	  protected internal AbstractIntermediateCatchEventBuilder(IBpmnModelInstance modelInstance, INtermediateCatchEvent element) : base(modelInstance, element)
	  {
	  }
	}

}