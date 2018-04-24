using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractSendTaskBuilder : AbstractTaskBuilder<ISendTask>, ISendTaskBuilder
    {

	  protected internal AbstractSendTaskBuilder(IBpmnModelInstance modelInstance, ISendTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the send task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder Implementation(string implementation)
	  {
		element.Implementation = implementation;
		return this;
	  }

	  /// <summary>
	  /// Sets the message of the send task. </summary>
	  /// <param name="message">  the message to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder Message(IMessage message)
	  {
		element.Message = message;
		return this;
	  }

	  /// <summary>
	  /// Sets the message with the given message name. If already a message
	  /// with this name exists it will be used, otherwise a new message is created.
	  /// </summary>
	  /// <param name="messageName"> the name of the message </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder Message(string messageName)
	  {
		IMessage message = FindMessageForName(messageName);
		return Message(message);
	  }

	  /// <summary>
	  /// Sets the operation of the send task.
	  /// </summary>
	  /// <param name="operation">  the operation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder Operation(IOperation operation)
	  {
		element.Operation = operation;
		return this;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaClass(string camundaClass)
	  {
		element.CamundaClass = camundaClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda delegateExpression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the delegateExpression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaDelegateExpression(string camundaExpression)
	  {
		element.CamundaDelegateExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda expression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaExpression(string camundaExpression)
	  {
		element.CamundaExpression = camundaExpression;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda resultVariable attribute.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the name of the process variable </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute.
	  /// </summary>
	  /// <param name="camundaTopic">  the topic to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return this;
	  }

	  /// <summary>
	  /// Set the camunda task priority attribute.
	  /// The priority is only used for service tasks which have as type value
	  /// <code>external</code>
	  /// </summary>
	  /// <param name="taskPriority"> the task priority which should used for the external tasks </param>
	  /// <returns> the builder object </returns>
	  public virtual ISendTaskBuilder CamundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return this;
	  }
	}

}