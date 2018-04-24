using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractThrowEventBuilder<TE> : AbstractEventBuilder<TE>, IThrowEventBuilder<TE> where TE : IThrowEvent
	{

	  protected internal AbstractThrowEventBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets an event definition for the given message name. If already a message
	  /// with this name exists it will be used, otherwise a new message is created.
	  /// </summary>
	  /// <param name="messageName"> the name of the message </param>
	  /// <returns> the builder object </returns>
	  public virtual IThrowEventBuilder<TE> Message(string messageName)
	  {
		IMessageEventDefinition messageEventDefinition = CreateMessageEventDefinition(messageName);
		element.EventDefinitions.Add(messageEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Creates an empty message event definition with an unique id
	  /// and returns a builder for the message event definition.
	  /// </summary>
	  /// <returns> the message event definition builder object </returns>
	  public virtual MessageEventDefinitionBuilder MessageEventDefinition()
	  {
		return MessageEventDefinition(null);
	  }

	  /// <summary>
	  /// Creates an empty message event definition with the given id
	  /// and returns a builder for the message event definition.
	  /// </summary>
	  /// <param name="id"> the id of the message event definition </param>
	  /// <returns> the message event definition builder object </returns>
	  public virtual MessageEventDefinitionBuilder MessageEventDefinition(string id)
	  {
		IMessageEventDefinition messageEventDefinition = CreateEmptyMessageEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  messageEventDefinition.Id = id;
		}

		element.EventDefinitions.Add(messageEventDefinition);
		return new MessageEventDefinitionBuilder(modelInstance, messageEventDefinition);
	  }

	  /// <summary>
	  /// Sets an event definition for the given signal name. If already a signal
	  /// with this name exists it will be used, otherwise a new signal is created.
	  /// </summary>
	  /// <param name="signalName"> the name of the signal </param>
	  /// <returns> the builder object </returns>
	  public virtual IThrowEventBuilder<TE> Signal(string signalName)
	  {
		ISignalEventDefinition signalEventDefinition = CreateSignalEventDefinition(signalName);
		element.EventDefinitions.Add(signalEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an
	  /// escalation with this code exists it will be used, otherwise a new
	  /// escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual IThrowEventBuilder<TE> Escalation(string escalationCode)
	  {
		IEscalationEventDefinition escalationEventDefinition = CreateEscalationEventDefinition(escalationCode);
		element.EventDefinitions.Add(escalationEventDefinition);

		return this;
	  }

	  public virtual CompensateEventDefinitionBuilder CompensateEventDefinition()
	  {
		return CompensateEventDefinition(null);
	  }

	  public virtual CompensateEventDefinitionBuilder CompensateEventDefinition(string id)
	  {
		ICompensateEventDefinition eventDefinition = CreateInstance<ICompensateEventDefinition>(typeof(ICompensateEventDefinition));
		if (!string.ReferenceEquals(id, null))
		{
		  eventDefinition.Id = id;
		}

		element.EventDefinitions.Add(eventDefinition);
		return new CompensateEventDefinitionBuilder(modelInstance, eventDefinition);
	  }
	}

}