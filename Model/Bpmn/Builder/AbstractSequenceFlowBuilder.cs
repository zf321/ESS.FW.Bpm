using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractSequenceFlowBuilder : AbstractFlowElementBuilder<ISequenceFlow>
	{

	  protected internal AbstractSequenceFlowBuilder(IBpmnModelInstance modelInstance, ISequenceFlow element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the source flow node of this sequence flow.
	  /// </summary>
	  /// <param name="source">  the source of this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractSequenceFlowBuilder From(IFlowNode source)
	  {
		element.Source = source;
		source.Outgoing.Add(element);
		return this;
	  }

	  /// <summary>
	  /// Sets the target flow node of this sequence flow.
	  /// </summary>
	  /// <param name="target">  the target of this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractSequenceFlowBuilder To(IFlowNode target)
	  {
		element.Target = target;
		target.Incoming.Add(element);
		return this;
	  }

	  /// <summary>
	  /// Sets the condition for this sequence flow.
	  /// </summary>
	  /// <param name="conditionExpression">  the condition expression for this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractSequenceFlowBuilder Condition(IConditionExpression conditionExpression)
	  {
		element.ConditionExpression = conditionExpression;
		return this;
	  }

	}

}