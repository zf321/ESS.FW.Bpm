using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// 
    /// <summary>
    /// 
    /// </summary>

    public abstract class AbstractMessageEventDefinitionBuilder : AbstractRootElementBuilder<IMessageEventDefinition>, IMessageEventDefinitionBuilder
    {

        public AbstractMessageEventDefinitionBuilder(IBpmnModelInstance modelInstance, IMessageEventDefinition element) : base(modelInstance, element)
        {
        }

        public override IMessageEventDefinitionBuilder Id<IMessageEventDefinitionBuilder>(string identifier)
        {
            return base.Id<IMessageEventDefinitionBuilder>(identifier);
        }

        /// <summary>
        /// Sets the message attribute.
        /// </summary>
        /// <param name="message"> the message for the message event definition </param>
        /// <returns> the builder object </returns>
        public virtual IMessageEventDefinitionBuilder Message(string message)
        {
            element.Message = FindMessageForName(message);
            return this;
        }

        /// <summary>
        /// Sets the camunda topic attribute. This is only meaningful when
        /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
        /// </summary>
        /// <param name="camundaTopic"> the topic to set </param>
        /// <returns> the builder object </returns>
        public virtual IMessageEventDefinitionBuilder CamundaTopic(string camundaTopic)
        {
            element.CamundaTopic = camundaTopic;
            return this;
        }

        /// <summary>
        /// Sets the camunda type attribute.
        /// </summary>
        /// <param name="camundaType">  the type of the service task </param>
        /// <returns> the builder object </returns>
        public virtual IMessageEventDefinitionBuilder CamundaType(string camundaType)
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
        public virtual IMessageEventDefinitionBuilder CamundaTaskPriority(string taskPriority)
        {
            element.CamundaTaskPriority = taskPriority;
            return this;
        }

        /// <summary>
        /// Finishes the building of a message event definition.
        /// </summary>
        /// @param <T> </param>
        /// <returns> the parent event builder </returns>
        public virtual IEventBuilder<IEvent> MessageEventDefinitionDone() /*where T : AbstractFlowNodeBuilder*/
        {
            return ((IEvent)element.ParentElement).Builder<IEventBuilder<IEvent>,IEvent>();
        }
    }
}