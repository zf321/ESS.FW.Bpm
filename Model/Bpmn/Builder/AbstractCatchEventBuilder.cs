using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractCatchEventBuilder<TE> : AbstractEventBuilder<TE>, ICatchEventBuilder<TE> where TE : ICatchEvent
	{

	  protected internal AbstractCatchEventBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the event to be parallel multiple
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> ParallelMultiple()
	  {
		//element.ParallelMultiple;
		return this;
	  }

	  /// <summary>
	  /// Sets an event definition for the given message name. If already a message
	  /// with this name exists it will be used, otherwise a new message is created.
	  /// </summary>
	  /// <param name="messageName"> the name of the message </param>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> Message(string messageName)
	  {
		IMessageEventDefinition messageEventDefinition = CreateMessageEventDefinition(messageName);
		element.EventDefinitions.Add(messageEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an event definition for the given signal name. If already a signal
	  /// with this name exists it will be used, otherwise a new signal is created.
	  /// </summary>
	  /// <param name="signalName"> the name of the signal </param>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> Signal(string signalName)
	  {
		ISignalEventDefinition signalEventDefinition = CreateSignalEventDefinition(signalName);
		element.EventDefinitions.Add(signalEventDefinition);

		return this;
	  }


	  /// <summary>
	  /// Sets an event definition for the timer with a time date.
	  /// </summary>
	  /// <param name="timerDate"> the time date of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> TimerWithDate(string timerDate)
	  {
		ITimeDate timeDate = CreateInstance<ITimeDate>(typeof(ITimeDate));
		timeDate.TextContent = timerDate;

		ITimerEventDefinition timerEventDefinition = CreateInstance<ITimerEventDefinition>(typeof(ITimerEventDefinition));
		timerEventDefinition.TimeDate = timeDate;

		element.EventDefinitions.Add(timerEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an event definition for the timer with a time duration.
	  /// </summary>
	  /// <param name="timerDuration"> the time duration of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> TimerWithDuration(string timerDuration)
	  {
		ITimeDuration timeDuration = CreateInstance<ITimeDuration>(typeof(ITimeDuration));
		timeDuration.TextContent = timerDuration;

		ITimerEventDefinition timerEventDefinition = CreateInstance<ITimerEventDefinition>(typeof(ITimerEventDefinition));
		timerEventDefinition.TimeDuration = timeDuration;

		element.EventDefinitions.Add(timerEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an event definition for the timer with a time cycle.
	  /// </summary>
	  /// <param name="timerCycle"> the time cycle of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual ICatchEventBuilder<TE> TimerWithCycle(string timerCycle)
	  {
		ITimeCycle timeCycle = CreateInstance<ITimeCycle>(typeof(ITimeCycle));
		timeCycle.TextContent = timerCycle;

		ITimerEventDefinition timerEventDefinition = CreateInstance<ITimerEventDefinition>(typeof(ITimerEventDefinition));
		timerEventDefinition.TimeCycle = timeCycle;

		element.EventDefinitions.Add(timerEventDefinition);

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

	  public virtual ConditionalEventDefinitionBuilder ConditionalEventDefinition()
	  {
		return ConditionalEventDefinition(null);
	  }

	  public virtual ConditionalEventDefinitionBuilder ConditionalEventDefinition(string id)
	  {
		IConditionalEventDefinition eventDefinition = CreateInstance<IConditionalEventDefinition>(typeof(IConditionalEventDefinition));
		if (!string.ReferenceEquals(id, null))
		{
		  eventDefinition.Id = id;
		}

		element.EventDefinitions.Add(eventDefinition);
		return new ConditionalEventDefinitionBuilder(modelInstance, eventDefinition);
	  }

	  public virtual ICatchEventBuilder<TE> Condition(string condition)
	  {
		ConditionalEventDefinition().Condition(condition);
		return this;
	  }

	}

}