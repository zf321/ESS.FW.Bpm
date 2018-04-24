using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// 
	/// <summary>
	/// </summary>

	public abstract class AbstractErrorEventDefinitionBuilder : AbstractRootElementBuilder<IErrorEventDefinition>,IErrorEventDefinitionBuilder
    {

	  public AbstractErrorEventDefinitionBuilder(IBpmnModelInstance modelInstance, IErrorEventDefinition element) : base(modelInstance, element)
	  {
	  }

	  public override IErrorEventDefinitionBuilder Id<IErrorEventDefinitionBuilder>(string identifier)
	  {
		return base.Id<IErrorEventDefinitionBuilder>(identifier);
	  }

	  /// <summary>
	  /// Sets the error code variable attribute.
	  /// </summary>
	  public virtual IErrorEventDefinitionBuilder ErrorCodeVariable(string errorCodeVariable)
	  {
		element.CamundaErrorCodeVariable = errorCodeVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the error message variable attribute.
	  /// </summary>
	  public virtual IErrorEventDefinitionBuilder ErrorMessageVariable(string errorMessageVariable)
	  {
		element.CamundaErrorMessageVariable = errorMessageVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the error attribute with errorCode.
	  /// </summary>
	  public virtual IErrorEventDefinitionBuilder Error(string errorCode)
	  {
		element.Error = FindErrorForNameAndCode(errorCode);
		return this;
	  }

        /// <summary>
        /// Finishes the building of a error event definition.
        /// </summary>
        /// @param <T> </param>
        /// <returns> the parent event builder </returns>
        public virtual IEventBuilder<IEvent> ErrorEventDefinitionDone() /*where T : AbstractFlowNodeBuilder*/
        {
            return ((IEvent)element.ParentElement).Builder<IEventBuilder<IEvent>,IEvent>();
        }
    }

}