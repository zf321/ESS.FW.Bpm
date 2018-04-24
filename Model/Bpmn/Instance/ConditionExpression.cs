

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN conditionExpression element of the BPMN tSequenceFlow type
	/// 
	/// 
	/// </summary>
	public interface IConditionExpression : IFormalExpression
	{

	  string Type {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaResource {get;set;}


	}

}