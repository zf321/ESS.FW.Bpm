using System;
using ESS.FW.Bpm.Model.Bpmn.builder;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using CallActivityBuilder = CallActivityBuilder;

	/// <summary>
	/// The BPMN callActivity element
	/// 
	/// 
	/// </summary>
	public interface ICallActivity : IActivity
	{

	  CallActivityBuilder Builder();

	  string CalledElement {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}


	  string CamundaCalledElementBinding {get;set;}


	  string CamundaCalledElementVersion {get;set;}


	  string CamundaCaseRef {get;set;}


	  string CamundaCaseBinding {get;set;}


	  string CamundaCaseVersion {get;set;}


	  string CamundaCalledElementTenantId {get;set;}


	  string CamundaCaseTenantId {get;set;}


	  string CamundaVariableMappingClass {get;set;}


	  string CamundaVariableMappingDelegateExpression {get;set;}


	}

}