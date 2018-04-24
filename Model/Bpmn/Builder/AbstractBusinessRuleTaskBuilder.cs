using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractBusinessRuleTaskBuilder : AbstractTaskBuilder<IBusinessRuleTask>
	{

	  protected internal AbstractBusinessRuleTaskBuilder(IBpmnModelInstance modelInstance, IBusinessRuleTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the business rule task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder Implementation(string implementation)
	  {
		element.Implementation = implementation;
		return this;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaClass(string camundaClass)
	  {
		element.CamundaClass = camundaClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda delegateExpression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the delegateExpression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaDelegateExpression(string camundaExpression)
	  {
		element.CamundaDelegateExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda expression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaExpression(string camundaExpression)
	  {
		element.CamundaExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda resultVariable attribute.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the name of the process variable </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute. This is only meaningful when
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRef attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRef"> the decisionRef to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaDecisionRef(string camundaDecisionRef)
	  {
		element.CamundaDecisionRef = camundaDecisionRef;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefBinding attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRefBinding"> the decisionRefBinding to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaDecisionRefBinding(string camundaDecisionRefBinding)
	  {
		element.CamundaDecisionRefBinding = camundaDecisionRefBinding;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefVersion attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRefVersion"> the decisionRefVersion to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaDecisionRefVersion(string camundaDecisionRefVersion)
	  {
		element.CamundaDecisionRefVersion = camundaDecisionRefVersion;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefTenantId attribute.
	  /// </summary>
	  /// <param name="decisionRefTenantId"> the decisionRefTenantId to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaDecisionRefTenantId(string decisionRefTenantId)
	  {
		element.CamundaDecisionRefTenantId = decisionRefTenantId;
		return this;
	  }

	  /// <summary>
	  /// Set the camunda mapDecisionResult attribute.
	  /// </summary>
	  /// <param name="camundaMapDecisionResult"> the mapper for the decision result to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaMapDecisionResult(string camundaMapDecisionResult)
	  {
		element.CamundaMapDecisionResult = camundaMapDecisionResult;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda task priority attribute. This is only meaningful when
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
	  /// 
	  /// </summary>
	  /// <param name="taskPriority"> the priority for the external task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBusinessRuleTaskBuilder CamundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return this;
	  }
	}

}