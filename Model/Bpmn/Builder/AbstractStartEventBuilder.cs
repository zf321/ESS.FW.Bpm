using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractStartEventBuilder : AbstractCatchEventBuilder<IStartEvent>
	{

	  protected internal AbstractStartEventBuilder(IBpmnModelInstance modelInstance, IStartEvent element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use camundaAsyncBefore() instead.
	  /// 
	  /// Sets the camunda async attribute to true.
	  /// 
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore() instead.")]
	  public virtual AbstractStartEventBuilder CamundaAsync()
	  {
		element.CamundaAsyncBefore = true;
		return this;
	  }

	  /// @deprecated use camundaAsyncBefore(isCamundaAsyncBefore) instead.
	  /// 
	  /// Sets the camunda async attribute.
	  /// 
	  /// <param name="isCamundaAsync">  the async state of the task </param>
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore(isCamundaAsyncBefore) instead.")]
	  public virtual AbstractStartEventBuilder CamundaAsync(bool isCamundaAsync)
	  {
		element.CamundaAsyncBefore = isCamundaAsync;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda form handler class attribute.
	  /// </summary>
	  /// <param name="camundaFormHandlerClass">  the class name of the form handler </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder camundaFormHandlerClass(string camundaFormHandlerClass)
	  {
		element.CamundaFormHandlerClass = camundaFormHandlerClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda form key attribute.
	  /// </summary>
	  /// <param name="camundaFormKey">  the form key to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder CamundaFormKey(string camundaFormKey)
	  {
		element.CamundaFormKey = camundaFormKey;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda initiator attribute.
	  /// </summary>
	  /// <param name="camundaInitiator">  the initiator to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder CamundaInitiator(string camundaInitiator)
	  {
		element.CamundaInitiator = camundaInitiator;
		return this;
	  }

	  /// <summary>
	  /// Creates a new camunda form field extension element.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual CamundaStartEventFormFieldBuilder camundaFormField()
	  {
		ICamundaFormData camundaFormData = GetCreateSingleExtensionElement<ICamundaFormData>(typeof(ICamundaFormData));
		ICamundaFormField camundaFormField = CreateChild<ICamundaFormField>(camundaFormData, typeof(ICamundaFormField));
		return new CamundaStartEventFormFieldBuilder(modelInstance, element, camundaFormField);
	  }

	  /// <summary>
	  /// Sets a catch all error definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder Error()
	  {
		IErrorEventDefinition errorEventDefinition = CreateInstance<IErrorEventDefinition>(typeof(IErrorEventDefinition));
		element.EventDefinitions.Add(errorEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder Error(string errorCode)
	  {
		IErrorEventDefinition errorEventDefinition = CreateErrorEventDefinition(errorCode);
		element.EventDefinitions.Add(errorEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Creates an error event definition with an unique id
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition(string id)
	  {
		IErrorEventDefinition errorEventDefinition = CreateEmptyErrorEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  errorEventDefinition.Id = id;
		}

		element.EventDefinitions.Add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Creates an error event definition
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition()
	  {
		IErrorEventDefinition errorEventDefinition = CreateEmptyErrorEventDefinition();
		element.EventDefinitions.Add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Sets a catch all escalation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder Escalation()
	  {
		IEscalationEventDefinition escalationEventDefinition = CreateInstance<IEscalationEventDefinition>(typeof(IEscalationEventDefinition));
		element.EventDefinitions.Add(escalationEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an escalation
	  /// with this code exists it will be used, otherwise a new escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder Escalation(string escalationCode)
	  {
		IEscalationEventDefinition escalationEventDefinition = CreateEscalationEventDefinition(escalationCode);
		element.EventDefinitions.Add(escalationEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets a catch compensation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual AbstractStartEventBuilder Compensation()
	  {
		ICompensateEventDefinition compensateEventDefinition = CreateCompensateEventDefinition();
		element.EventDefinitions.Add(compensateEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets whether the start event is interrupting or not.
	  /// </summary>
	  public virtual AbstractStartEventBuilder Interrupting(bool interrupting)
	  {
		element.Interrupting = interrupting;

		return this;
	  }

	}

}