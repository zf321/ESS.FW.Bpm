

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using SendTaskBuilder = SendTaskBuilder;

	/// <summary>
	/// The BPMN sendTask element
	/// 
	/// 
	/// </summary>
	public interface ISendTask : ITask
	{

	  SendTaskBuilder Builder();

	  string Implementation {get;set;}


	  IMessage Message {get;set;}


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