

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class UserTaskBuilder : AbstractUserTaskBuilder/*<UserTaskBuilder>*/,IBpmnModelElementBuilder<ICamundaFormField>
	{

	  public UserTaskBuilder(IBpmnModelInstance modelInstance, IUserTask element) : base(modelInstance, element)
	  {
	  }

	}

}