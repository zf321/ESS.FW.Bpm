

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

    /// <summary>
	/// The BPMN sequenceFlow element
	/// 
	/// 
	/// </summary>
	public interface ISequenceFlow : IFlowElement
	{

	  SequenceFlowBuilder Builder();

	  IFlowNode Source {get;set;}


	  IFlowNode Target {get;set;}


	  bool Immediate {get;set;}


	  IConditionExpression ConditionExpression {get;set;}


	  void RemoveConditionExpression();

	  new IBpmnEdge DiagramElement {get;}

	}

}