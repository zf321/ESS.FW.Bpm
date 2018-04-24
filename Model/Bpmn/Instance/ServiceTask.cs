

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ServiceTaskBuilder = ServiceTaskBuilder;

	/// <summary>
	/// The BPMN serviceTask element
	/// 
	/// 
	/// </summary>
	public interface IServiceTask : ITask
	{

	  ServiceTaskBuilder Builder();

	  string Implementation {get;set;}


	  IOperation Operation {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaClass {get;set;}


	  string CamundaDelegateExpression {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaResultVariable {get;set;}


	  string CamundaType {get;set;}


	  string CamundaTopic {get;set;}


	  string CamundaTaskPriority {get;set;}

	}

}