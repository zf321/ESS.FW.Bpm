

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using BusinessRuleTaskBuilder = BusinessRuleTaskBuilder;

	/// <summary>
	/// The BPMN businessRuleTask element
	/// 
	/// 
	/// </summary>
	public interface IBusinessRuleTask : ITask
	{

	  BusinessRuleTaskBuilder Builder();

	  string Implementation {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaClass {get;set;}


	  string CamundaDelegateExpression {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaResultVariable {get;set;}


	  string CamundaType {get;set;}


	  string CamundaTopic {get;set;}


	  string CamundaDecisionRef {get;set;}


	  string CamundaDecisionRefBinding {get;set;}


	  string CamundaDecisionRefVersion {get;set;}


	  string CamundaDecisionRefTenantId {get;set;}


	  string CamundaMapDecisionResult {get;set;}


	  string CamundaTaskPriority {get;set;}


	}

}