using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractReceiveTaskBuilder : AbstractTaskBuilder<IReceiveTask>, IReceiveTaskBuilder
    {

	  protected internal AbstractReceiveTaskBuilder(IBpmnModelInstance modelInstance, IReceiveTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the receive task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IReceiveTaskBuilder Implementation(string implementation)
	  {
		element.Implementation = implementation;
		return this;
	  }

	  /// <summary>
	  /// Sets the receive task instantiate attribute to true.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual IReceiveTaskBuilder Instantiate()
	  {
		element.Instantiate = true;
		return this;
	  }

	  /// <summary>
	  /// Sets the message of the send task. </summary>
	  /// <param name="message">  the message to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IReceiveTaskBuilder Message(IMessage message)
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
	  public virtual IReceiveTaskBuilder Message(string messageName)
	  {
		IMessage message = FindMessageForName(messageName);
		return Message(message);
	  }

	  /// <summary>
	  /// Sets the operation of the send task.
	  /// </summary>
	  /// <param name="operation">  the operation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IReceiveTaskBuilder Operation(IOperation operation)
	  {
		element.Operation = operation;
		return this;
	  }

	}

}