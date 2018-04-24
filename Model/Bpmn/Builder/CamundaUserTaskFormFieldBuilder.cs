

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class CamundaUserTaskFormFieldBuilder : AbstractCamundaFormFieldBuilder<UserTaskBuilder>
	{

	  public CamundaUserTaskFormFieldBuilder(IBpmnModelInstance modelInstance, IBaseElement parent, ICamundaFormField element) : base(modelInstance, parent, element)
	  {
	  }

	}

}