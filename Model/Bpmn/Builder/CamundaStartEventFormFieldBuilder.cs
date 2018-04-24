

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// @author Kristin Polenz
	/// 
	/// </summary>
	public class CamundaStartEventFormFieldBuilder : AbstractCamundaFormFieldBuilder<StartEventBuilder>
	{

	  public CamundaStartEventFormFieldBuilder(IBpmnModelInstance modelInstance, IBaseElement parent, ICamundaFormField element) : base(modelInstance, parent, element)
	  {
	  }

	}

}