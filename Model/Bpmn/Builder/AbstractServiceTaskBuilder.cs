using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractServiceTaskBuilder: AbstractTaskBuilder<IServiceTask> 
	{

	  protected internal AbstractServiceTaskBuilder(IBpmnModelInstance modelInstance, IServiceTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the build service task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder Implementation(string implementation)
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
	  public virtual AbstractServiceTaskBuilder CamundaClass(string camundaClass)
	  {
		element.CamundaClass = camundaClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda delegateExpression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the delegateExpression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaDelegateExpression(string camundaExpression)
	  {
		element.CamundaDelegateExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda expression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaExpression(string camundaExpression)
	  {
		element.CamundaExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda resultVariable attribute.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the name of the process variable </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute. This is only meaningful when
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the build object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda task priority attribute. This is only meaningful when
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
	  /// 
	  /// </summary>
	  /// <param name="taskPriority"> the priority for the external task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractServiceTaskBuilder CamundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return this;
	  }
	}

}