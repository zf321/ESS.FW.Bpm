

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class BusinessRuleTaskBuilder : AbstractBusinessRuleTaskBuilder/*<BusinessRuleTaskBuilder>*/
	{

	  public BusinessRuleTaskBuilder(IBpmnModelInstance modelInstance, IBusinessRuleTask element) : base(modelInstance, element)
	  {
	  }
	}

}