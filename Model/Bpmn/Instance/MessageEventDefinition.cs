

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN messageEventDefinition element
	/// 
	/// 
	/// 
	/// </summary>
	public interface IMessageEventDefinition : IEventDefinition
	{

	  IMessage Message {get;set;}


	  IOperation Operation {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaClass {get;set;}


	  string CamundaDelegateExpression {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaResultVariable {get;set;}


	  string CamundaTopic {get;set;}


	  string CamundaType {get;set;}


	  string CamundaTaskPriority {get;set;}


	}

}