

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class StartEventBuilder : AbstractStartEventBuilder ,IBpmnModelElementBuilder<ICamundaFormField>//<StartEventBuilder>
	{
      public StartEventBuilder(IBpmnModelInstance modelInstance, IStartEvent element) : base(modelInstance, element)
	  {
	  }
	}

}