using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// Represents the abstract conditional event definition builder.
	///  </summary>
	/// @param <B> </param>
	public class AbstractConditionalEventDefinitionBuilder : AbstractRootElementBuilder<IConditionalEventDefinition>, IConditionalEventDefinitionBuilder
    {

	  public AbstractConditionalEventDefinitionBuilder(IBpmnModelInstance modelInstance, IConditionalEventDefinition element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the condition of the conditional event definition.
	  /// </summary>
	  /// <param name="conditionText"> the condition which should be evaluate to true or false </param>
	  /// <returns> the builder object </returns>
	  public virtual IConditionalEventDefinitionBuilder Condition(string conditionText)
	  {
		ICondition condition = CreateInstance<ICondition>(typeof(ICondition));
		condition.TextContent = conditionText;
		element.Condition = condition;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda variable name attribute, that defines on
	  /// which variable the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableName"> the variable on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual IConditionalEventDefinitionBuilder CamundaVariableName(string variableName)
	  {
		element.CamundaVariableName = variableName;
		return this;
	  }

	  /// <summary>
	  /// Set the camunda variable events attribute, that defines the variable
	  /// event on which the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableEvents"> the events on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual IConditionalEventDefinitionBuilder CamundaVariableEvents(string variableEvents)
	  {
		element.CamundaVariableEvents = variableEvents;
		return this;
	  }

	  /// <summary>
	  /// Set the camunda variable events attribute, that defines the variable
	  /// event on which the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableEvents"> the events on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual IConditionalEventDefinitionBuilder CamundaVariableEvents(IList<string> variableEvents)
	  {
		element.CamundaVariableEventsList = variableEvents;
		return this;
	  }

        /// <summary>
        /// Finishes the building of a conditional event definition.
        /// </summary>
        /// @param <T> </param>
        /// <returns> the parent event builder </returns>
        public virtual IEventBuilder<IEvent> ConditionalEventDefinitionDone() /*where T : AbstractFlowNodeBuilder*/
        {
            return ((IEvent)element.ParentElement).Builder<IEventBuilder<IEvent>,IEvent>();
        }

    }
}